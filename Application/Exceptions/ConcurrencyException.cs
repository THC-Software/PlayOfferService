namespace PlayOfferService.Application.Exceptions;

public class ConcurrencyException(string message) : Exception(message);