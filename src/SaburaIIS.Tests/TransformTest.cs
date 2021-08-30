using Microsoft.Web.Administration;
using SaburaIIS.Agent;
using SaburaIIS.Agent.Mappers;
using SaburaIIS.Agent.Transformers;
using System;
using System.Linq;
using Xunit;

namespace SaburaIIS.Tests
{
    public class TransformTest
    {
        [Fact]
        public void Json()
        {
            using var manager = new ServerManager();
            var locals = new Mapper().Map<Site, POCO.Site>(manager.Sites).ToList();
            var json = Helpers.ToJson(locals);
            Console.WriteLine(json);
        }
        [Fact]
        public void AddApplicationPool()
        {
            using var manager = new ServerManager();
            var locals = new Mapper().Map<ApplicationPool, POCO.ApplicationPool>(manager.ApplicationPools).ToList();
            var remotes = Helpers.Clone(locals);

            var poolName = "Add";

            remotes.Add(new POCO.ApplicationPool
            {
                Name = poolName,
                AutoStart = true,
                Enable32BitAppOnWin64 = false,
                ManagedPipelineMode = POCO.ManagedPipelineMode.Integrated,
                ManagedRuntimeVersion = "v4.0",
                QueueLength = 2000,
                StartMode = POCO.StartMode.OnDemand,
                Cpu = new POCO.ApplicationPoolCpu
                {
                    Action = POCO.ProcessorAction.Throttle,
                    Limit = 20,
                    ResetInterval = TimeSpan.FromMilliseconds(300_001),
                    SmpAffinitized = false,
                    SmpProcessorAffinityMask = 0xffffffff,
                    SmpProcessorAffinityMask2 = 0xffffffff,
                },
                Failure = new POCO.ApplicationPoolFailure
                {
                    AutoShutdownExe = "",
                    AutoShutdownParams = "",
                    LoadBalancerCapabilities = POCO.LoadBalancerCapabilities.HttpLevel,
                    OrphanActionExe = "",
                    OrphanActionParams = "",
                    OrphanWorkerProcess = false,
                    RapidFailProtection = true,
                    RapidFailProtectionInterval = TimeSpan.FromMilliseconds(300_001),
                    RapidFailProtectionMaxCrashes = 5,
                },
                ProcessModel = new POCO.ApplicationPoolProcessModel
                {
                    IdentityType = POCO.ProcessModelIdentityType.ApplicationPoolIdentity,
                    IdleTimeout = TimeSpan.FromMilliseconds(1200_001),
                    IdleTimeoutAction = POCO.IdleTimeoutAction.Terminate,
                    LoadUserProfile = false,
                    MaxProcesses = 1,
                    PingingEnabled = true,
                    PingInterval = TimeSpan.FromMilliseconds(300_001),
                    PingResponseTime = TimeSpan.FromMilliseconds(900_001),
                    Password = "",
                    ShutdownTimeLimit = TimeSpan.FromMilliseconds(900_001),
                    StartupTimeLimit = TimeSpan.FromMilliseconds(900_001),
                    UserName = "",
                    LogEventOnProcessModel = POCO.ProcessModelLogEventOnProcessModel.IdleTimeout,
                },
                Recycling = new POCO.ApplicationPoolRecycling
                {
                    DisallowOverlappingRotation = false,
                    DisallowRotationOnConfigChange = false,
                    LogEventOnRecycle =
                        POCO.RecyclingLogEventOnRecycle.PrivateMemory
                        | POCO.RecyclingLogEventOnRecycle.ConfigChange
                        | POCO.RecyclingLogEventOnRecycle.OnDemand
                        | POCO.RecyclingLogEventOnRecycle.IsapiUnhealthy
                        | POCO.RecyclingLogEventOnRecycle.Memory
                        | POCO.RecyclingLogEventOnRecycle.Schedule
                        | POCO.RecyclingLogEventOnRecycle.Requests
                        | POCO.RecyclingLogEventOnRecycle.Time,
                    PeriodicRestart = new POCO.ApplicationPoolPeriodicRestart
                    {
                        Memory = 100,
                        PrivateMemory = 101,
                        Requests = 102,
                        Schedule = new [] {
                            new POCO.Schedule
                            {
                                Time = TimeSpan.FromHours(6)
                            }
                        },
                        Time = TimeSpan.FromMilliseconds(104400_001),
                    }
                }
            });

            var deltas = Delta.CreateCollection(locals, remotes).ToList();

            Assert.Single(deltas);

            var delta = deltas.First();

            Assert.Equal(DeltaMethod.Add, delta.Method);

            Transformer.Transform(manager.ApplicationPools, delta);

            Assert.Equal(remotes.Count, manager.ApplicationPools.Count);

            var pool = manager.ApplicationPools.FirstOrDefault(pool => pool.Name == poolName);

            Assert.NotNull(pool);

            Assert.True(pool.AutoStart);
            Assert.False(pool.Enable32BitAppOnWin64);
            Assert.Equal(ManagedPipelineMode.Integrated, pool.ManagedPipelineMode);
            Assert.Equal("v4.0", pool.ManagedRuntimeVersion);
            Assert.Equal(2000, pool.QueueLength);
            Assert.Equal(StartMode.OnDemand, pool.StartMode);
            Assert.Equal(ProcessorAction.Throttle, pool.Cpu.Action);
            Assert.Equal(20, pool.Cpu.Limit);
            Assert.Equal(TimeSpan.FromMilliseconds(300_001), pool.Cpu.ResetInterval);
            Assert.False(pool.Cpu.SmpAffinitized);
            Assert.Equal(0xffffffff, pool.Cpu.SmpProcessorAffinityMask);
            Assert.Equal(0xffffffff, pool.Cpu.SmpProcessorAffinityMask2);
            Assert.Equal("", pool.Failure.AutoShutdownExe);
            Assert.Equal("", pool.Failure.AutoShutdownParams);
            Assert.Equal(LoadBalancerCapabilities.HttpLevel, pool.Failure.LoadBalancerCapabilities);
            Assert.Equal("", pool.Failure.OrphanActionExe);
            Assert.Equal("", pool.Failure.OrphanActionParams);
            Assert.False(pool.Failure.OrphanWorkerProcess);
            Assert.True(pool.Failure.RapidFailProtection);
            Assert.Equal(TimeSpan.FromMilliseconds(300_001), pool.Failure.RapidFailProtectionInterval);
            Assert.Equal(5, pool.Failure.RapidFailProtectionMaxCrashes);
            Assert.Equal(5, pool.Failure.RapidFailProtectionMaxCrashes);
            Assert.Equal(5, pool.Failure.RapidFailProtectionMaxCrashes);
            Assert.Equal(5, pool.Failure.RapidFailProtectionMaxCrashes);
            Assert.Equal(ProcessModelIdentityType.ApplicationPoolIdentity, pool.ProcessModel.IdentityType);
            Assert.Equal(TimeSpan.FromMilliseconds(1200_001), pool.ProcessModel.IdleTimeout);
            Assert.Equal(IdleTimeoutAction.Terminate, pool.ProcessModel.IdleTimeoutAction);
            Assert.False(pool.ProcessModel.LoadUserProfile);
            Assert.Equal(1, pool.ProcessModel.MaxProcesses);
            Assert.True(pool.ProcessModel.PingingEnabled);
            Assert.Equal(TimeSpan.FromMilliseconds(300_001), pool.ProcessModel.PingInterval);
            Assert.Equal(TimeSpan.FromMilliseconds(900_001), pool.ProcessModel.PingResponseTime);
            Assert.Equal("", pool.ProcessModel.Password);
            Assert.Equal(TimeSpan.FromMilliseconds(900_001), pool.ProcessModel.ShutdownTimeLimit);
            Assert.Equal(TimeSpan.FromMilliseconds(900_001), pool.ProcessModel.StartupTimeLimit);
            Assert.Equal("", pool.ProcessModel.UserName);
            Assert.Equal(ProcessModelLogEventOnProcessModel.IdleTimeout, pool.ProcessModel.LogEventOnProcessModel);
            Assert.False(pool.Recycling.DisallowOverlappingRotation);
            Assert.False(pool.Recycling.DisallowRotationOnConfigChange);
            Assert.Equal(RecyclingLogEventOnRecycle.PrivateMemory
                        | RecyclingLogEventOnRecycle.ConfigChange
                        | RecyclingLogEventOnRecycle.OnDemand
                        | RecyclingLogEventOnRecycle.IsapiUnhealthy
                        | RecyclingLogEventOnRecycle.Memory
                        | RecyclingLogEventOnRecycle.Schedule
                        | RecyclingLogEventOnRecycle.Requests
                        | RecyclingLogEventOnRecycle.Time, pool.Recycling.LogEventOnRecycle);
            Assert.Equal(100, pool.Recycling.PeriodicRestart.Memory);
            Assert.Equal(101, pool.Recycling.PeriodicRestart.PrivateMemory);
            Assert.Equal(102, pool.Recycling.PeriodicRestart.Requests);
            Assert.NotEmpty(pool.Recycling.PeriodicRestart.Schedule);
            Assert.Equal(TimeSpan.FromHours(6), pool.Recycling.PeriodicRestart.Schedule.First().Time);
            Assert.Equal(TimeSpan.FromMilliseconds(104400_001), pool.Recycling.PeriodicRestart.Time);
        }

    }
}
