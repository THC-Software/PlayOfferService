namespace PlayOfferService.Domain.Events;

public enum EventType
{
    PLAYOFFER_CREATED,
    PLAYOFFER_JOINED,
    PLAYOFFER_CANCELLED,
    PLAYOFFER_OPPONENT_REMOVED,
    PLAYOFFER_RESERVATION_ADDED,
    TENNIS_CLUB_REGISTERED,
    TENNIS_CLUB_LOCKED,
    TENNIS_CLUB_UNLOCKED,
    TENNIS_CLUB_DELETED,
    MEMBER_REGISTERED,
    MEMBER_LOCKED,
    MEMBER_UNLOCKED,
    MEMBER_DELETED,
    MEMBER_UPDATED,
    ReservationCreatedEvent,
    ReservationRejectedEvent,
    ReservationLimitExceeded,
    ReservationCancelledEvent,
    CourtCreatedEvent,
    CourtUpdatedEvent,
}