 
 


using System;
using System.Collections.Generic;

namespace SaburaIIS.POCO
{
	public partial class Application
	{
		public string ApplicationPoolName { get; set; }
		public string EnabledProtocols { get; set; }
		public string Path { get; set; }
		// VirtualDirectoryCollection
		public ICollection<VirtualDirectory> VirtualDirectories { get; set; }
		// public VirtualDirectoryDefaults VirtualDirectoryDefaults { get; set; }
	}
	public partial class ApplicationDefaults
	{
		public string ApplicationPoolName { get; set; }
		public string EnabledProtocols { get; set; }
	}
	public partial class ApplicationDomain
	{
		public string Id { get; set; }
		public Int32 Idle { get; set; }
		public string PhysicalPath { get; set; }
		public string VirtualPath { get; set; }
		public WorkerProcess WorkerProcess { get; set; }
	}
	public partial class ApplicationPool
	{
		public Boolean AutoStart { get; set; }
		public ApplicationPoolCpu Cpu { get; set; }
		public Boolean Enable32BitAppOnWin64 { get; set; }
		public Boolean EnableEmulationOnWinArm64 { get; set; }
		public ApplicationPoolFailure Failure { get; set; }
		public ManagedPipelineMode ManagedPipelineMode { get; set; }
		public StartMode StartMode { get; set; }
		public string ManagedRuntimeVersion { get; set; }
		public string Name { get; set; }
		public ApplicationPoolProcessModel ProcessModel { get; set; }
		public Int64 QueueLength { get; set; }
		public ApplicationPoolRecycling Recycling { get; set; }
		public ObjectState State { get; set; }
		// WorkerProcessCollection
		public ICollection<WorkerProcess> WorkerProcesses { get; set; }
	}
	public partial class ApplicationPoolCpu
	{
		public ProcessorAction Action { get; set; }
		public Int64 Limit { get; set; }
		public TimeSpan ResetInterval { get; set; }
		public Boolean SmpAffinitized { get; set; }
		public Int64 SmpProcessorAffinityMask { get; set; }
		public Int64 SmpProcessorAffinityMask2 { get; set; }
	}
	public partial class ApplicationPoolDefaults
	{
		public Boolean AutoStart { get; set; }
		public ApplicationPoolCpu Cpu { get; set; }
		public Boolean Enable32BitAppOnWin64 { get; set; }
		public Boolean EnableEmulationOnWinArm64 { get; set; }
		public ApplicationPoolFailure Failure { get; set; }
		public ManagedPipelineMode ManagedPipelineMode { get; set; }
		public StartMode StartMode { get; set; }
		public string ManagedRuntimeVersion { get; set; }
		public ApplicationPoolProcessModel ProcessModel { get; set; }
		public Int64 QueueLength { get; set; }
		public ApplicationPoolRecycling Recycling { get; set; }
	}
	public partial class ApplicationPoolFailure
	{
		public string AutoShutdownExe { get; set; }
		public string AutoShutdownParams { get; set; }
		public LoadBalancerCapabilities LoadBalancerCapabilities { get; set; }
		public string OrphanActionExe { get; set; }
		public string OrphanActionParams { get; set; }
		public Boolean OrphanWorkerProcess { get; set; }
		public Boolean RapidFailProtection { get; set; }
		public TimeSpan RapidFailProtectionInterval { get; set; }
		public Int64 RapidFailProtectionMaxCrashes { get; set; }
	}
	public partial class ApplicationPoolPeriodicRestart
	{
		public Int64 Memory { get; set; }
		public Int64 PrivateMemory { get; set; }
		public Int64 Requests { get; set; }
		// ScheduleCollection
		public ICollection<Schedule> Schedule { get; set; }
		public TimeSpan Time { get; set; }
	}
	public partial class ApplicationPoolProcessModel
	{
		public ProcessModelIdentityType IdentityType { get; set; }
		public TimeSpan IdleTimeout { get; set; }
		public IdleTimeoutAction IdleTimeoutAction { get; set; }
		public Boolean LoadUserProfile { get; set; }
		public Int64 MaxProcesses { get; set; }
		public Boolean PingingEnabled { get; set; }
		public TimeSpan PingInterval { get; set; }
		public TimeSpan PingResponseTime { get; set; }
		public string Password { get; set; }
		public TimeSpan ShutdownTimeLimit { get; set; }
		public TimeSpan StartupTimeLimit { get; set; }
		public string UserName { get; set; }
		public ProcessModelLogEventOnProcessModel LogEventOnProcessModel { get; set; }
	}
	public partial class ApplicationPoolRecycling
	{
		public Boolean DisallowOverlappingRotation { get; set; }
		public Boolean DisallowRotationOnConfigChange { get; set; }
		public RecyclingLogEventOnRecycle LogEventOnRecycle { get; set; }
		public ApplicationPoolPeriodicRestart PeriodicRestart { get; set; }
	}
	public partial class Binding
	{
		public string BindingInformation { get; set; }
		public Byte[] CertificateHash { get; set; }
		public string CertificateStoreName { get; set; }
		// public IPEndPoint EndPoint { get; set; }
		public string Host { get; set; }
		public Boolean IsIPPortHostBinding { get; set; }
		public SslFlags SslFlags { get; set; }
		public Boolean UseDsMapper { get; set; }
		public string Protocol { get; set; }
	}
	public partial class ConfigurationSection
	{
		public Boolean IsLocked { get; set; }
		public OverrideMode OverrideMode { get; set; }
		public OverrideMode OverrideModeEffective { get; set; }
		public string SectionPath { get; set; }
	}
	public partial class CustomLogField
	{
		public string LogFieldName { get; set; }
		public string SourceName { get; set; }
		public CustomLogFieldSourceType SourceType { get; set; }
	}
	public partial class Request
	{
		public string ClientIPAddr { get; set; }
		public string ConnectionId { get; set; }
		public string CurrentModule { get; set; }
		public string HostName { get; set; }
		public string LocalIPAddress { get; set; }
		public Int32 LocalPort { get; set; }
		public PipelineState PipelineState { get; set; }
		public Int32 ProcessId { get; set; }
		public string RequestId { get; set; }
		public Int32 SiteId { get; set; }
		public Int32 TimeElapsed { get; set; }
		public Int32 TimeInModule { get; set; }
		public Int32 TimeInState { get; set; }
		public string Url { get; set; }
		public string Verb { get; set; }
	}
	public partial class Schedule
	{
		public TimeSpan Time { get; set; }
	}
	public partial class Site
	{
		// public ApplicationDefaults ApplicationDefaults { get; set; }
		// ApplicationCollection
		public ICollection<Application> Applications { get; set; }
		// BindingCollection
		public ICollection<Binding> Bindings { get; set; }
		public Int64 Id { get; set; }
		public SiteLimits Limits { get; set; }
		public SiteLogFile LogFile { get; set; }
		public string Name { get; set; }
		public Boolean ServerAutoStart { get; set; }
		public ObjectState State { get; set; }
		public SiteTraceFailedRequestsLogging TraceFailedRequestsLogging { get; set; }
		public SiteHSTS HSTS { get; set; }
		// public VirtualDirectoryDefaults VirtualDirectoryDefaults { get; set; }
	}
	public partial class SiteDefaults
	{
		public SiteLimits Limits { get; set; }
		public SiteLogFile LogFile { get; set; }
		public Boolean ServerAutoStart { get; set; }
		public SiteTraceFailedRequestsLogging TraceFailedRequestsLogging { get; set; }
		public SiteHSTS HSTS { get; set; }
	}
	public partial class SiteLimits
	{
		public TimeSpan ConnectionTimeout { get; set; }
		public Int64 MaxBandwidth { get; set; }
		public Int64 MaxConnections { get; set; }
		public Int64 MaxUrlSegments { get; set; }
	}
	public partial class SiteLogFile
	{
		public string Directory { get; set; }
		public Boolean LocalTimeRollover { get; set; }
		public LogExtFileFlags LogExtFileFlags { get; set; }
		public Boolean Enabled { get; set; }
		// CustomLogFieldCollection
		public ICollection<CustomLogField> CustomLogFields { get; set; }
		public Guid CustomLogPluginClsid { get; set; }
		public LoggingRolloverPeriod Period { get; set; }
		public LogFormat LogFormat { get; set; }
		public LogTargetW3C LogTargetW3C { get; set; }
		public Int64 TruncateSize { get; set; }
	}
	public partial class SiteTraceFailedRequestsLogging
	{
		public string Directory { get; set; }
		public Boolean Enabled { get; set; }
		public Int64 MaxLogFiles { get; set; }
	}
	public partial class SiteHSTS
	{
		public Boolean Enabled { get; set; }
		public Int64 MaxAge { get; set; }
		public Boolean IncludeSubDomains { get; set; }
		public Boolean Preload { get; set; }
		public Boolean RedirectHttpToHttps { get; set; }
	}
	public partial class VirtualDirectory
	{
		public AuthenticationLogonMethod LogonMethod { get; set; }
		public string Password { get; set; }
		public string Path { get; set; }
		public string PhysicalPath { get; set; }
		public string UserName { get; set; }
	}
	public partial class VirtualDirectoryDefaults
	{
		public AuthenticationLogonMethod LogonMethod { get; set; }
		public string Password { get; set; }
		public string UserName { get; set; }
	}
	public partial class WorkerProcess
	{
		// ApplicationDomainCollection
		public ICollection<ApplicationDomain> ApplicationDomains { get; set; }
		public string AppPoolName { get; set; }
		public string ProcessGuid { get; set; }
		public Int32 ProcessId { get; set; }
		public WorkerProcessState State { get; set; }
	}
	public partial class String
	{
		public Char this[Int32 index]
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public Int32 Length { get; set; }
	}
	public partial class Object
	{
	}
	public partial class ConfigurationElementSchema
	{
		public Boolean AllowUnrecognizedAttributes { get; set; }
		// public ConfigurationAttributeSchemaCollection AttributeSchemas { get; set; }
		// public ConfigurationElementSchemaCollection ChildElementSchemas { get; set; }
		// public ConfigurationCollectionSchema CollectionSchema { get; set; }
		public Boolean IsCollectionDefault { get; set; }
		public string Name { get; set; }
	}
	public partial class ConfigurationCollectionSchema
	{
		public string AddElementNames { get; set; }
		public Boolean AllowDuplicates { get; set; }
		public string ClearElementName { get; set; }
		public Boolean IsMergeAppend { get; set; }
		public string RemoveElementName { get; set; }
	}
	public partial class ConfigurationAttributeSchema
	{
		public Boolean AllowInfinite { get; set; }
		// public Object DefaultValue { get; set; }
		public Boolean IsCaseSensitive { get; set; }
		public Boolean IsCombinedKey { get; set; }
		public Boolean IsEncrypted { get; set; }
		public Boolean IsExpanded { get; set; }
		public Boolean IsRequired { get; set; }
		public Boolean IsUniqueKey { get; set; }
		public string Name { get; set; }
		public string TimeSpanFormat { get; set; }
		public string Type { get; set; }
		public string ValidationParameter { get; set; }
		public string ValidationType { get; set; }
	}
	public partial class ConfigurationMethod
	{
		public string Name { get; set; }
		// public ConfigurationMethodSchema Schema { get; set; }
	}
	public partial class ConfigurationMethodSchema
	{
		// public ConfigurationElementSchema InputSchema { get; set; }
		public string Name { get; set; }
		// public ConfigurationElementSchema OutputSchema { get; set; }
	}
	public partial class ConfigurationElement
	{
		// public ConfigurationAttributeCollection Attributes { get; set; }
		// public ConfigurationChildElementCollection ChildElements { get; set; }
		public string ElementTagName { get; set; }
		public Boolean IsLocallyStored { get; set; }
		public Object this[String attributeName]
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		// public ConfigurationMethodCollection Methods { get; set; }
		public IDictionary<String,String> RawAttributes { get; set; }
		// public ConfigurationElementSchema Schema { get; set; }
	}
	public partial class ConfigurationAttribute
	{
		public Boolean IsInheritedFromDefaultValue { get; set; }
		public Boolean IsProtected { get; set; }
		public string Name { get; set; }
		// public ConfigurationAttributeSchema Schema { get; set; }
		// public Object Value { get; set; }
	}
	public partial class IPEndPoint
	{
		public AddressFamily AddressFamily { get; set; }
		// public IPAddress Address { get; set; }
		public Int32 Port { get; set; }
	}
	public partial class IPAddress
	{
		public Int64 Address { get; set; }
		public AddressFamily AddressFamily { get; set; }
		public Int64 ScopeId { get; set; }
		public Boolean IsIPv6Multicast { get; set; }
		public Boolean IsIPv6LinkLocal { get; set; }
		public Boolean IsIPv6SiteLocal { get; set; }
		public Boolean IsIPv6Teredo { get; set; }
		public Boolean IsIPv4MappedToIPv6 { get; set; }
	}
	public enum WorkerProcessState
	{
		Starting = 0,
		Running = 1,
		Stopping = 2,
		Unknown = 3,
	}
	public enum AuthenticationLogonMethod
	{
		Interactive = 0,
		Batch = 1,
		Network = 2,
		ClearText = 3,
	}
	public enum LogExtFileFlags
	{
		Date = 1,
		Time = 2,
		ClientIP = 4,
		UserName = 8,
		SiteName = 16,
		ComputerName = 32,
		ServerIP = 64,
		Method = 128,
		UriStem = 256,
		UriQuery = 512,
		HttpStatus = 1024,
		Win32Status = 2048,
		BytesSent = 4096,
		BytesRecv = 8192,
		TimeTaken = 16384,
		ServerPort = 32768,
		UserAgent = 65536,
		Cookie = 131072,
		Referer = 262144,
		ProtocolVersion = 524288,
		Host = 1048576,
		HttpSubStatus = 2097152,
	}
	public enum LoggingRolloverPeriod
	{
		MaxSize = 0,
		Daily = 1,
		Weekly = 2,
		Monthly = 3,
		Hourly = 4,
	}
	public enum LogFormat
	{
		Iis = 0,
		Ncsa = 1,
		W3c = 2,
		Custom = 3,
	}
	public enum LogTargetW3C
	{
		File = 1,
		ETW = 2,
	}
	public enum ObjectState
	{
		Starting = 0,
		Started = 1,
		Stopping = 2,
		Stopped = 3,
		Unknown = 4,
	}
	public enum PipelineState
	{
		Unknown = 0,
		BeginRequest = 1,
		AuthenticateRequest = 2,
		AuthorizeRequest = 4,
		ResolveRequestCache = 8,
		MapRequestHandler = 16,
		AcquireRequestState = 32,
		PreExecuteRequestHandler = 64,
		ExecuteRequestHandler = 128,
		ReleaseRequestState = 256,
		UpdateRequestCache = 512,
		LogRequest = 1024,
		EndRequest = 2048,
		SendResponse = 536870912,
	}
	public enum CustomLogFieldSourceType
	{
		RequestHeader = 0,
		ResponseHeader = 1,
		ServerVariable = 2,
	}
	public enum OverrideMode
	{
		Unknown = 0,
		Inherit = 1,
		Allow = 2,
		Deny = 3,
	}
	public enum SslFlags
	{
		None = 0,
		Sni = 1,
		CentralCertStore = 2,
		DisableHTTP2 = 4,
		DisableOCSPStp = 8,
		DisableQUIC = 16,
		DisableTLS13 = 32,
		DisableLegacyTLS = 64,
		NegotiateClientCert = 128,
	}
	public enum AddressFamily
	{
		Unspecified = 0,
		Unix = 1,
		InterNetwork = 2,
		ImpLink = 3,
		Pup = 4,
		Chaos = 5,
		Ipx = 6,
		NS = 6,
		Iso = 7,
		Osi = 7,
		Ecma = 8,
		DataKit = 9,
		Ccitt = 10,
		Sna = 11,
		DecNet = 12,
		DataLink = 13,
		Lat = 14,
		HyperChannel = 15,
		AppleTalk = 16,
		NetBios = 17,
		VoiceView = 18,
		FireFox = 19,
		Banyan = 21,
		Atm = 22,
		InterNetworkV6 = 23,
		Cluster = 24,
		Ieee12844 = 25,
		Irda = 26,
		NetworkDesigners = 28,
		Max = 29,
		Unknown = -1,
	}
	public enum RecyclingLogEventOnRecycle
	{
		None = 0,
		Time = 1,
		Requests = 2,
		Schedule = 4,
		Memory = 8,
		IsapiUnhealthy = 16,
		OnDemand = 32,
		ConfigChange = 64,
		PrivateMemory = 128,
	}
	public enum ProcessModelIdentityType
	{
		LocalSystem = 0,
		LocalService = 1,
		NetworkService = 2,
		SpecificUser = 3,
		ApplicationPoolIdentity = 4,
	}
	public enum IdleTimeoutAction
	{
		Terminate = 0,
		Suspend = 1,
	}
	public enum ProcessModelLogEventOnProcessModel
	{
		None = 0,
		IdleTimeout = 1,
	}
	public enum LoadBalancerCapabilities
	{
		TcpLevel = 1,
		HttpLevel = 2,
	}
	public enum ManagedPipelineMode
	{
		Integrated = 0,
		Classic = 1,
	}
	public enum StartMode
	{
		OnDemand = 0,
		AlwaysRunning = 1,
	}
	public enum ProcessorAction
	{
		NoAction = 0,
		KillW3wp = 1,
		Throttle = 2,
		ThrottleUnderLoad = 3,
	}
}
