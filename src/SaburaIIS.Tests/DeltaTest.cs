using SaburaIIS.Agent;
using System.Linq;
using Xunit;

namespace SaburaIIS.Tests
{
    public class DeltaTest
    {
        [Fact]
        public void DividePerMethods()
        {
            var locals = new[]
            {
                new POCO.ApplicationPool
                {
                    Name = "A"
                },
                new POCO.ApplicationPool
                {
                    Name = "B"
                }
            };
            var remotes = new[] 
            {
                new POCO.ApplicationPool
                {
                    Name = "B"
                },
                new POCO.ApplicationPool
                {
                    Name = "C"
                },
            };

            var deltas = Delta.CreateCollection(locals, remotes).ToList();

            Assert.Equal(3, deltas.Count());
            Assert.Equal(DeltaMethod.Remove, deltas.Where(d => (string)d.Key == "A").First().Method);
            Assert.Equal(DeltaMethod.Update, deltas.Where(d => (string)d.Key == "B").First().Method);
            Assert.Equal(DeltaMethod.Add,    deltas.Where(d => (string)d.Key == "C").First().Method);
        }

        [Fact]
        public void UpdateDeltaIsEmptyWhenPoolValuesAreSame()
        {
            var local = new POCO.ApplicationPool
            {
                Name = "A"
            };
            var remote = new POCO.ApplicationPool
            {
                Name = "A"
            };
            var delta = Delta.Create(local, remote);
            Assert.Empty(delta.ValueProperties);
            Assert.Empty(delta.NestProperties);
            Assert.Empty(delta.NestCollectionProperties);
        }

        [Fact]
        public void UpdateDeltaHasValuePropDiffs()
        {
            var local = new POCO.ApplicationPool
            {
                Name = "A",
                AutoStart = false,
                Enable32BitAppOnWin64 = false,
            };
            var remote = new POCO.ApplicationPool
            {
                Name = "A",
                AutoStart = true,
                Enable32BitAppOnWin64 = true,
            };
            var delta = Delta.Create(local, remote);
            Assert.Equal(2, delta.ValueProperties.Count);
            Assert.Equal("AutoStart", delta.ValueProperties.ElementAt(0).Key);
            Assert.Equal(true, delta.ValueProperties.ElementAt(0).Value.newValue);
            Assert.Equal("Enable32BitAppOnWin64", delta.ValueProperties.ElementAt(1).Key);
            Assert.Equal(true, delta.ValueProperties.ElementAt(1).Value.newValue);
        }

        [Fact]
        public void UpdateDeltaHasClassPropDiffs()
        {
            var local = new POCO.ApplicationPool
            {
                Name = "A",
                Cpu = new POCO.ApplicationPoolCpu
                {
                    Action = POCO.ProcessorAction.Throttle
                }
            };
            var remote = new POCO.ApplicationPool
            {
                Name = "A",
                Cpu = new POCO.ApplicationPoolCpu
                {
                    Action = POCO.ProcessorAction.ThrottleUnderLoad
                }
            };
            var delta = Delta.Create(local, remote);
            Assert.Empty(delta.ValueProperties);
            Assert.Empty(delta.NestCollectionProperties);
            Assert.NotEmpty(delta.NestProperties);
            Assert.Equal("Cpu", delta.NestProperties.First().Key);
            delta = delta.NestProperties.First().Value;
            Assert.NotEmpty(delta.ValueProperties);
            Assert.Equal("Action", delta.ValueProperties.First().Key);
            Assert.Equal(POCO.ProcessorAction.ThrottleUnderLoad, delta.ValueProperties.First().Value.newValue);
        }

        [Fact]
        public void AdditionDeltaHasClassProps()
        {
            var locals = new[]
            {
                new POCO.ApplicationPool
                {
                    Name = "A"
                },
            };
            var remotes = new[]
            {
                new POCO.ApplicationPool
                {
                    Name = "B",
                    Cpu = new POCO.ApplicationPoolCpu
                    {
                        Action = POCO.ProcessorAction.ThrottleUnderLoad,
                    }
                },
            };

            var deltas = Delta.CreateCollection(locals, remotes).ToList();
            var delta = deltas.Where(d => (string)d.Key == "B").First();
            Assert.Equal(DeltaMethod.Add, delta.Method);
            delta = delta.NestProperties.First().Value;
            Assert.Equal(DeltaMethod.Add, delta.Method);
            Assert.NotEmpty(delta.ValueProperties);
            Assert.Empty(delta.NestProperties);
            Assert.Empty(delta.NestCollectionProperties);
            Assert.Contains("Action", delta.ValueProperties.Keys);
            Assert.Contains("Limit", delta.ValueProperties.Keys);
            Assert.Contains("ResetInterval", delta.ValueProperties.Keys);
            Assert.Contains("SmpAffinitized", delta.ValueProperties.Keys);
            Assert.Contains("SmpProcessorAffinityMask", delta.ValueProperties.Keys);
            Assert.Contains("SmpProcessorAffinityMask2", delta.ValueProperties.Keys);
        }

        [Fact]
        public void RemoveDeltaHasClassProps()
        {
            var locals = new[]
            {
                new POCO.ApplicationPool
                {
                    Name = "A",
                    Cpu = new POCO.ApplicationPoolCpu
                    {
                        Action = POCO.ProcessorAction.ThrottleUnderLoad,
                    }
                },
            };
            var remotes = new[]
            {
                new POCO.ApplicationPool
                {
                    Name = "B"
                },
            };

            var deltas = Delta.CreateCollection(locals, remotes).ToList();
            var delta = deltas.Where(d => (string)d.Key == "A").First();
            Assert.Equal(DeltaMethod.Remove, delta.Method);
            delta = delta.NestProperties.First().Value;
            Assert.Equal(DeltaMethod.Remove, delta.Method);
            Assert.NotEmpty(delta.ValueProperties);
            Assert.Empty(delta.NestProperties);
            Assert.Empty(delta.NestCollectionProperties);
            Assert.Contains("Action", delta.ValueProperties.Keys);
            Assert.Contains("Limit", delta.ValueProperties.Keys);
            Assert.Contains("ResetInterval", delta.ValueProperties.Keys);
            Assert.Contains("SmpAffinitized", delta.ValueProperties.Keys);
            Assert.Contains("SmpProcessorAffinityMask", delta.ValueProperties.Keys);
            Assert.Contains("SmpProcessorAffinityMask2", delta.ValueProperties.Keys);
        }
    }
}
