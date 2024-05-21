namespace PlayOfferService.Domain.Repositories;

public class CQRSContexts
{
    public ReadDbContext ReadContext { get; }
    public WriteDbContext WriteContext { get; }

    public CQRSContexts(ReadDbContext _readContext, WriteDbContext _writeContext)
    {
        ReadContext = _readContext;
        WriteContext = _writeContext;
    }
}
