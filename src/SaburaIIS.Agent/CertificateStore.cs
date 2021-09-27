using SaburaIIS.Agent.Extensions;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class CertificateStore : IDisposable
{
    private X509Store? _store;
    public CertificateStore(string storeName)
    {
        _store = new X509Store(storeName, StoreLocation.LocalMachine);
        _store.Open(OpenFlags.ReadWrite);
    }
    public void Dispose()
    {
        if (_store != null)
        {
            _store.Close();
            _store.Dispose();
            _store = null;
        }
    }
    public virtual bool Contains(string thumbprint)
    {
        return _store!.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false).Count > 0;
    }
    public virtual void Add(byte[] rawData)
    {
        _store!.Add(new X509Certificate2(rawData, (string?)null, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet));
    }
}

public class CertificateStoreFactory
{
    public virtual CertificateStore Create(string storeName)
    {
        return new CertificateStore(storeName);
    }
}