using Moq;
using SaburaIIS.Agent;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SaburaIIS.Tests
{
    public class AgentWorkerTest
    {
        [Fact]
        public async Task DeployCertificatesTest1()
        {
            var certificates = new[]
            {
                new Models.Certificate
                {
                    Name = "test123",
                    Thumbprint = new byte[]{ 1, 2, 3 }
                },
                new Models.Certificate
                {
                    Name = "test456",
                    Thumbprint = new byte[]{ 4, 5, 6 }
                }
            };
            var test456RawData = new byte[] { 1, 0, 1, 0 };
            var config = new Agent.Config { ResourceGroupName = "saburaiis" };
            var mKeyVault = new Mock<KeyVault>(config);
            mKeyVault.Setup(m => m.GetCertificatesAsync()).Returns(() => Task.FromResult(certificates.Take(2)));
            mKeyVault.Setup(m => m.GetCertificateAsync("test456", null)).Returns(() => Task.FromResult(test456RawData));

            var mMyCertStore = new Mock<CertificateStore>("My");
            mMyCertStore.Setup(m => m.Contains(It.IsAny<string>())).Returns((string thumbprint) => 
            {
                return thumbprint == Convert.ToHexString(certificates[0].Thumbprint);
            });

            var added = 0;
            mMyCertStore.Setup(m => m.Add(It.IsAny<byte[]>())).Callback((byte[] b) =>
            {
                Assert.True(b.SequenceEqual(test456RawData));
                added++;
            });

            var mCertStoreFactory = new Mock<CertificateStoreFactory>();
            mCertStoreFactory.Setup(m => m.Create("My")).Returns(mMyCertStore.Object);

            var model = new WorkerModel(config, null, null, mKeyVault.Object, null, mCertStoreFactory.Object, null, null, Helpers.CreateTestLogger<WorkerModel>());

            await model.DeployCertificates(new Models.Partition
            {
                Sites = new[]
                {
                    new POCO.Site
                    {
                        Bindings = new[]
                        {
                            new POCO.Binding
                            {
                                CertificateHash = new byte[]{ 1, 2, 3 },
                                CertificateStoreName = "My"
                            },
                            new POCO.Binding
                            {
                                CertificateHash = new byte[]{ 4, 5, 6 },
                                CertificateStoreName = "My"
                            }
                        }
                    },
                    new POCO.Site
                    {
                        Bindings = new[]
                        {
                            new POCO.Binding
                            {
                                CertificateHash = new byte[]{ 4, 5, 6 },
                                CertificateStoreName = "My"
                            }
                        }
                    }
                }
            });

            Assert.Equal(1, added);
        }

        [Fact]
        public async Task DeployPackageTest1()
        {
            var testPhysicalPath = @"%temp%\test\v0.0.1-alpha.1";
            var testPhysicalPathExpanded = Environment.ExpandEnvironmentVariables(testPhysicalPath);
            if (Directory.Exists(testPhysicalPathExpanded))
                Directory.Delete(testPhysicalPathExpanded, true);

            Assert.False(Directory.Exists(testPhysicalPathExpanded));

            var testContent = "<html><body>test</body></html>";
            using var stream = new MemoryStream();
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            using (var writer = new StreamWriter(zip.CreateEntry("index.html").Open()))
            {
                writer.Write(testContent);
            }

            stream.Seek(0, SeekOrigin.Begin);

            var config = new Agent.Config { ResourceGroupName = "saburaiis" };

            var mStorage = new Mock<Storage>(config);
            mStorage.Setup(m => m.DownloadAsync(It.IsAny<string>())).Returns(() => Task.FromResult<Stream>(stream));

            var mStore = new Mock<Store>(config);
            mStore.Setup(m => m.GetReleaseAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.FromResult(new Models.Release { }));

            var model = new WorkerModel(config, mStore.Object, mStorage.Object, null, null, null, null, null, Helpers.CreateTestLogger<WorkerModel>());

            await model.DeployPackages(new[] {
                new Delta
                {
                    Key = "Default Web Site",
                    NestCollectionProperties =
                    {
                        [nameof(POCO.Site.Applications)] = new []
                        {
                            new Delta
                            {
                                Key = "/",
                                NestCollectionProperties =
                                {
                                    [nameof(POCO.Application.VirtualDirectories)] = new[]
                                    {
                                        new Delta
                                        {
                                            Key = "/",
                                            ValueProperties =
                                            {
                                                [nameof(POCO.VirtualDirectory.PhysicalPath)] = (testPhysicalPath, null)
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            Assert.True(Directory.Exists(testPhysicalPathExpanded));
            var testContentFile = Path.Combine(testPhysicalPathExpanded, "index.html");
            Assert.True(File.Exists(testContentFile));
            Assert.Equal(testContent, File.ReadAllText(testContentFile));
        }

        [Fact]
        public async Task ExecuteAsyncTest1()
        {
            var config = new Agent.Config { 
                ResourceGroupName = "saburaiis",
                ScaleSetName = "test",
                PoolingDelaySecForInit = 0,
                PoolingDelaySecForAssign = 0,
                PoolingDelaySecForUpdate = 0,
            };
            var partition = new Models.Partition
            { 
                ScaleSets = new []{ new Models.VirtualMachineScaleSet { Name = config.ScaleSetName } }
            };
            var mTracker = new Mock<ChangeTracker<Models.Partition>>(null);
            mTracker.Setup(m => m.HasChangeAsync()).Returns(Task.FromResult(true));

            var mStore = new Mock<Store>(config);
            mStore.Setup(m => m.InitAsync()).Returns(Task.CompletedTask);
            mStore.Setup(m => m.SearchPartitionAsync(config.ScaleSetName)).Returns(Task.FromResult("test-partition"));
            mStore.Setup(m => m.CreatePartitionChangeTracker(It.IsAny<string>())).Returns(mTracker.Object);
            mStore.Setup(m => m.GetPartitionAsync(It.IsAny<string>())).Returns(Task.FromResult((partition, "*")));

            var mReporter = new Mock<ServerConfigWatcher>(Helpers.CreateTestLogger<ServerConfigWatcher>(), "%temp%");
            mReporter.Setup(m => m.StartWatchAsync(It.IsAny<Func<Task>>())).Returns(Task.CompletedTask);

            var model = new Mock<WorkerModel>(config, mStore.Object, null, null, null, null, null, mReporter.Object, Helpers.CreateTestLogger<WorkerModel>());

            var stoppingTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var updateIsCalled = false;
            model.Setup(m => m.Update(It.IsAny<Models.Partition>())).Returns(() =>
            {
                updateIsCalled = true;
                stoppingTokenSource.Cancel();
                return Task.CompletedTask;
            });

            try
            {
                await model.Object.ExecuteAsync(stoppingTokenSource.Token);
            }
            catch (TaskCanceledException ex) {
                Console.WriteLine(ex);
            }

            Assert.True(updateIsCalled);
        }

        [Fact]
        public async Task ExecuteAsyncTest2()
        {
            var config = new Agent.Config
            {
                ResourceGroupName = "saburaiis",
                ScaleSetName = "test",
                PoolingDelaySecForInit = 0,
                PoolingDelaySecForAssign = 0,
                PoolingDelaySecForUpdate = 0,
            };
            var partition = new Models.Partition
            {
                ScaleSets = new[] { new Models.VirtualMachineScaleSet { Name = config.ScaleSetName } }
            };
            var mTracker = new Mock<ChangeTracker<Models.Partition>>(null);
            mTracker.Setup(m => m.HasChangeAsync()).Returns(Task.FromResult(true));

            var mStore = new Mock<Store>(config);
            var initCount = 0;
            mStore.Setup(m => m.InitAsync()).Returns(() => {
                if (++initCount == 3)
                    return Task.CompletedTask;
                throw new InvalidOperationException();
            });
            mStore.Setup(m => m.SearchPartitionAsync(config.ScaleSetName)).Returns(Task.FromResult("test-partition"));
            mStore.Setup(m => m.CreatePartitionChangeTracker(It.IsAny<string>())).Returns(mTracker.Object);
            mStore.Setup(m => m.GetPartitionAsync(It.IsAny<string>())).Returns(Task.FromResult((partition, "*")));

            var mReporter = new Mock<ServerConfigWatcher>(Helpers.CreateTestLogger<ServerConfigWatcher>(), "%temp%");
            mReporter.Setup(m => m.StartWatchAsync(It.IsAny<Func<Task>>())).Returns(Task.CompletedTask);

            var model = new Mock<WorkerModel>(config, mStore.Object, null, null, null, null, null, mReporter.Object, Helpers.CreateTestLogger<WorkerModel>());

            var stoppingTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var updateIsCalled = false;
            model.Setup(m => m.Update(It.IsAny<Models.Partition>())).Returns(() =>
            {
                updateIsCalled = true;
                stoppingTokenSource.Cancel();
                return Task.CompletedTask;
            });

            try
            {
                await model.Object.ExecuteAsync(stoppingTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex);
            }

            Assert.True(updateIsCalled);
            Assert.Equal(3, initCount);
        }

        [Fact]
        public async Task ExecuteAsyncTest3()
        {
            var config = new Agent.Config
            {
                ResourceGroupName = "saburaiis",
                ScaleSetName = "test",
                PoolingDelaySecForInit = 0,
                PoolingDelaySecForAssign = 0,
                PoolingDelaySecForUpdate = 0,
            };
            var partition = new Models.Partition
            {
                ScaleSets = new[] { new Models.VirtualMachineScaleSet { Name = config.ScaleSetName } }
            };
            var mTracker = new Mock<ChangeTracker<Models.Partition>>(null);
            mTracker.Setup(m => m.HasChangeAsync()).Returns(Task.FromResult(true));

            var mStore = new Mock<Store>(config);
            var assignCount = 0;
            mStore.Setup(m => m.InitAsync()).Returns(Task.CompletedTask);
            mStore.Setup(m => m.SearchPartitionAsync(config.ScaleSetName)).Returns(() =>
            {
                if (++assignCount == 3)
                    return Task.FromResult("test-partition");

                return Task.FromResult<string>(null);
            });
            mStore.Setup(m => m.CreatePartitionChangeTracker(It.IsAny<string>())).Returns(mTracker.Object);
            mStore.Setup(m => m.GetPartitionAsync(It.IsAny<string>())).Returns(Task.FromResult((partition, "*")));

            var mReporter = new Mock<ServerConfigWatcher>(Helpers.CreateTestLogger<ServerConfigWatcher>(), "%temp%");
            mReporter.Setup(m => m.StartWatchAsync(It.IsAny<Func<Task>>())).Returns(Task.CompletedTask);

            var model = new Mock<WorkerModel>(config, mStore.Object, null, null, null, null, null, mReporter.Object, Helpers.CreateTestLogger<WorkerModel>());

            var stoppingTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var updateIsCalled = false;
            model.Setup(m => m.Update(It.IsAny<Models.Partition>())).Returns(() =>
            {
                updateIsCalled = true;
                stoppingTokenSource.Cancel();
                return Task.CompletedTask;
            });

            try
            {
                await model.Object.ExecuteAsync(stoppingTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex);
            }

            Assert.True(updateIsCalled);
            Assert.Equal(3, assignCount);
        }
    }
}
