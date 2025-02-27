﻿using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Mappers
{
    public class ApplicationPoolDefaultsMapper : IMapper<ApplicationPoolDefaults, POCO.ApplicationPoolDefaults>
    {
        public void Map(ApplicationPoolDefaults src, POCO.ApplicationPoolDefaults dst)
        {
            dst.ManagedRuntimeVersion = src.ManagedRuntimeVersion;
            dst.QueueLength = src.QueueLength;
            dst.AutoStart = src.AutoStart;
            dst.Enable32BitAppOnWin64 = src.Enable32BitAppOnWin64;
            dst.Cpu = new POCO.ApplicationPoolCpu
            {
                Action = (POCO.ProcessorAction)(int)src.Cpu.Action,
                SmpAffinitized = src.Cpu.SmpAffinitized,
                SmpProcessorAffinityMask = src.Cpu.SmpProcessorAffinityMask,
                SmpProcessorAffinityMask2 = src.Cpu.SmpProcessorAffinityMask2,
                Limit = src.Cpu.Limit,
                ResetInterval = src.Cpu.ResetInterval,
            };
            dst.Failure = new POCO.ApplicationPoolFailure
            {
                AutoShutdownExe = src.Failure.AutoShutdownExe,
                AutoShutdownParams = src.Failure.AutoShutdownParams,
                OrphanActionExe = src.Failure.OrphanActionExe,
                OrphanActionParams = src.Failure.OrphanActionParams,
                OrphanWorkerProcess = src.Failure.OrphanWorkerProcess,
                RapidFailProtection = src.Failure.RapidFailProtection,
                RapidFailProtectionInterval = src.Failure.RapidFailProtectionInterval,
                RapidFailProtectionMaxCrashes = src.Failure.RapidFailProtectionMaxCrashes,
                LoadBalancerCapabilities = (POCO.LoadBalancerCapabilities)(int)src.Failure.LoadBalancerCapabilities
            };
            dst.ManagedPipelineMode = (POCO.ManagedPipelineMode)(int)src.ManagedPipelineMode;
            dst.ProcessModel = new POCO.ApplicationPoolProcessModel
            {
                IdentityType = (POCO.ProcessModelIdentityType)(int)src.ProcessModel.IdentityType,
                IdleTimeout = src.ProcessModel.IdleTimeout,
                LoadUserProfile = src.ProcessModel.LoadUserProfile,
                MaxProcesses = src.ProcessModel.MaxProcesses,
                PingingEnabled = src.ProcessModel.PingingEnabled,
                PingInterval = src.ProcessModel.PingInterval,
                PingResponseTime = src.ProcessModel.PingResponseTime,
                Password = src.ProcessModel.Password,
                ShutdownTimeLimit = src.ProcessModel.ShutdownTimeLimit,
                StartupTimeLimit = src.ProcessModel.StartupTimeLimit,
                UserName = src.ProcessModel.UserName,
                LogEventOnProcessModel = (POCO.ProcessModelLogEventOnProcessModel)(int)src.ProcessModel.LogEventOnProcessModel,
            };
            dst.Recycling = new POCO.ApplicationPoolRecycling
            {
                DisallowOverlappingRotation = src.Recycling.DisallowOverlappingRotation,
                DisallowRotationOnConfigChange = src.Recycling.DisallowRotationOnConfigChange,
                LogEventOnRecycle = (POCO.RecyclingLogEventOnRecycle)(int)src.Recycling.LogEventOnRecycle,
                PeriodicRestart = new POCO.ApplicationPoolPeriodicRestart
                {
                    Memory = src.Recycling.PeriodicRestart.Memory,
                    PrivateMemory = src.Recycling.PeriodicRestart.PrivateMemory,
                    Requests = src.Recycling.PeriodicRestart.Requests,
                    Schedule = src.Recycling.PeriodicRestart.Schedule.Select(schedule => new POCO.Schedule
                    {
                        Time = schedule.Time
                    }).ToList(),
                    Time = src.Recycling.PeriodicRestart.Time
                }
            };
        }

        public POCO.ApplicationPoolDefaults Map(ApplicationPoolDefaults src)
        {
            var dst = new POCO.ApplicationPoolDefaults();
            Map(src, dst);
            return dst;
        }

        public IEnumerable<POCO.ApplicationPoolDefaults> Map(IEnumerable<ApplicationPoolDefaults> sources)
        {
            return sources.Select(Map);
        }
    }
}
