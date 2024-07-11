# PlayOfferService

## Deploying Kubernetes Manifests Locally with Minikube

### Prerequisites

- [Install Minikube](https://minikube.sigs.k8s.io/docs/start/)
- [Install kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)

### Step-by-Step Guide

#### 1. Start Minikube

Start Minikube with sufficient resources.

```bash
minikube start --memory=4096 --cpus=2
```

#### 2. Apply kubernetes Manifests

Use kubectl to apply each of these YAML files. This will create the necessary Kubernetes resources.

```bash
kubectl apply -f redis.yml
kubectl apply -f pos_postgres_write.yml
kubectl apply -f pos_postgres_read.yml
kubectl apply -f pos_debezium.yml
kubectl apply -f pos_service.yml
```

#### 3. Check the status of pods/services

Check the status of your pods and services to ensure they are running correctly.

```bash
kubectl get pods
kubectl get services
```

#### 4. Access the service

Minikube provides a way to access services running inside the cluster using minikube service.

```bash
minikube service pos-service
```

You can also use `kubectl port-forward` to forward a port from your local machine to a port on a pod. For example:

```bash
kubectl port-forward deployment/pos-service 8080:8080
```

## API Documentation
We provided a OpenAPI documentation for the PlayOfferService.
It can be found in the [`openapi.json`](./openapi.json) file.

## Used patterns in microservice
**All `.cs` files are linked to the respective file in the project**

### Saga
The Saga pattern is used to maintain data consistency in a microservice architecture. It is a sequence of local transactions where each transaction updates the data and publishes a message or event to trigger the next transaction in the sequence. If a transaction fails, the saga executes a series of compensating transactions that undo the changes that were made by the preceding transactions.

In the PlayOfferService the Saga Pattern is used in conjunction with the CourtService, to automatically create a Reservation if a PlayOffer is joined by a second Member. It includes the following Steps:

1. After `JoinPlayOfferCommand` is received, the PlayOfferService publishes a `PlayOfferJoinedEvent`
2. The CourtService listens to the `PlayOfferJoinedEvent` and tries to create a Reservation for the PlayOffer at the specified time in the PlayOfferJoinedEvent
3. The CourtService publishes one of three possible events, each containing the `EventId` of the `PlayOfferJoinedEvent` in the `CorrelationId`:
   - `ReservationCreatedEvent` if the Reservation was successfully created
   - `ReservationRejectedEvent` if the Reservation could not be created (e.g. no court available)
   - `ReservationLimitExceededEvent` if the Reservation could not be created due to a limit of Reservations per Member
4. The PlayOfferService listens to the events published by the CourtService and reacts depending on the event:
   - If a `ReservationCreatedEvent` is received it then triggers a `PlayOfferReservationAddedEvent` in the PlayOfferService to add the Reservation to the respective PlayOffer
   - If a `ReservationRejectedEvent` or `ReservationLimitExceededEvent` is received it then triggers a `PlayOfferOpponentRemovedEvent` to revert the changes of the `PlayOfferJoinedEvent`

The **compensation logic** for the Saga is implemented in the [ReservationEventHandler](./Application/Handlers/Events/ReservationEventHandler.cs) File in the functions in lines _81 - 102_.

### CQRS
The CQRS pattern is used in the PlayOfferService to separate the read and write operations for PlayOffers. The write operations are implemented using commands, which are located in the [Commands](./Application/Commands) folder. The read operations are implemented using queries, which are located in the [Queries](./Application/Queries) folder.
Each query and command is then handled by their respective handlers, which are located in the root of the [Handlers](./Application/Handlers) folder. Each handler is responsible for executing the logic for a specific command or query.

#### Queries
The following queries are implemented in the PlayOfferService with their respective handlers:
- [`GetPlayOffersByClubIdQuery.cs`](./Application/Queries/GetPlayOffersByClubIdQuery.cs)(_Line 1:8_): Returns all PlayOffers for a specific Club. The query is created and sent to the handler in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Line 40_).
  - [`GetPlayOffersByClubIdHandler.cs`](./Application/Handlers/GetPlayOffersByClubIdHandler.cs)(_Line 1:49_): Handles the `GetPlayOffersByClubIdQuery` and returns the PlayOffers for the specified Club
- [`GetPlayOffersByParticipantIdQuery.cs`](./Application/Queries/GetPlayOffersByParticipantIdQuery.cs)(_Line 1:8_): Returns all PlayOffers for a specific participant (either as creator or opponent). The query is created and sent to the handler in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Line 64_).
  - [`GetPlayOffersByParticipantIdHandler.cs`](./Application/Handlers/GetPlayOffersByParticipantIdHandler.cs)(_Line 1:49_): Handles the `GetPlayOffersByParticipantIdQuery` and returns the PlayOffers for the specified participant
- [`GetPlayOffersByCreatorNameQuery.cs`](./Application/Queries/GetPlayOffersByCreatorNameQuery.cs)(_Line 1:8_): Returns a specific PlayOffer by the name of it's creator. The query is created and sent to the handler in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Line 91_).
  - [`GetPlayOffersByCreatorNameHandler.cs`](./Application/Handlers/GetPlayOffersByCreatorNameHandler.cs)(_Line 1:59_): Handles the `GetPlayOfferByCreatorNameQuery` and returns the PlayOffer with the specified Id

#### Commands
The following commands are implemented in the PlayOfferService with their respective handlers:
- [`CancelPlayOfferCommand.cs`](./Application/Commands/CancelPlayOfferCommand.cs)(_Line 1:7_): Cancels a PlayOffer. The command is created and sent to the handler in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Line 158_).
  - [`CancelPlayOfferHandler.cs`](./Application/Handlers/CancelPlayOfferHandler.cs)(_Line 1:79_): Handles the `CancelPlayOfferCommand` and cancels the PlayOffer
- [`CreatePlayOfferCommand.cs`](./Application/Commands/CreatePlayOfferCommand.cs)(_Line 1:7_): Creates a new PlayOffer. The command is created and sent to the handler in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Line 128_).
  - [`CreatePlayOfferHandler.cs`](./Application/Handlers/CreatePlayOfferHandler.cs)(_Line 1:87_): Handles the `CreatePlayOfferCommand` and creates a new 
- [`JoinPlayOfferCommand.cs`](./Application/Commands/JoinPlayOfferCommand.cs)(_Line 1:7_): Joins a PlayOffer. The command is created and sent to the handler in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Line 192_).
  - [`JoinPlayOfferHandler.cs`](./Application/Handlers/JoinPlayOfferHandler.cs)(_Line 1:100_): Handles the `JoinPlayOfferCommand` and joins the PlayOffer


#### Projection
In the PlayOfferService, projections are implemented using the **Mediator Pattern** which is implemented, in dedicated `EventHandlers` for each entity, in the [Events](./Application/Handlers/Events) folder.

Each Entity has a dedicated `RedisStreamReader` which subscribes to the Redis stream and listens to, filters and parses the events for a specific entity:
- [`PlayOfferEventHandler.cs`](./Application/Handlers/Events/PlayOfferEventHandler.cs)(_Line 1:94_): Handles the events for the `PlayOffer` entity
- [`MemberEventHandler.cs`](./Application/Handlers/Events/MemberEventHandler.cs)(_Line 1:126_): Handles the events for the `Member` entity
- [`ReservationEventHandler.cs`](./Application/Handlers/Events/ReservationEventHandler.cs)(_Line 1:151_): Handles the events for the `Reservation` entity
- [`CourtEventHandler.cs`](./Application/Handlers/Events/CourtEventHandler.cs)(_Line 1:57_): Handles the events for the `Court` entity
- [`ClubEventHandler.cs`](./Application/Handlers/Events/ClubEventHandler.cs)(_Line 1:116_): Handles the events for the `Club` entity

The `EventHandlers` receive their events from the `RedisStreamService` and then apply the events to the respective entity:
- [`RedisClubStreamService.cs`](./Application/RedisClubStreamService.cs)(_Line 1:82_): Read the events from the redis club stream and sends them to the `ClubEventHandler`
- [`RedisCourtStreamService.cs`](./Application/RedisCourtStreamService.cs)(_Line 1:83_): Read the events from the redis court stream and sends them to the `CourtEventHandler`
- [`RedisMemberStreamService.cs`](./Application/RedisMemberStreamService.cs)(_Line 1:86_): Read the events from the redis member stream and sends them to the `MemberEventHandler`
- [`RedisPlayOfferStreamService.cs`](./Application/RedisPlayOfferStreamService.cs)(_Line 1:68_): Read the events from the redis play offer stream and sends them to the `PlayOfferEventHandler`
- [`RedisReservationStreamService.cs`](./Application/RedisReservationStreamService.cs)(_Line 1:76_): Read the events from the redis reservation stream and sends them to the `ReservationEventHandler`

### Event Sourcing
The write side of the CQRS implementation is using a event sourcing pattern. In the PlayOfferService, events are used to represent changes to the state of Entities.
When a command is received, it is validated and then converted into one or more events, which are then stored in the write side database.

The events are structured with a hierarchy of event classes:
- `Technical[...]Event`: Represents a group of events that are used for a specific entity, these are used to route the events to the correct `EventHandler` in the read model. Implements the `BaseEvent` class.
    - [`TechnicalPlayOfferEvent.cs`](./Domain/Events/PlayOffer/TechnicalPlayOfferEvent.cs)(_Line 1:7_): Represents the events for the `PlayOffer` entity
    - [`TechnicalMemberEvent.cs`](./Domain/Events/Member/TechnicalMemberEvent.cs)(_Line 1:7_): Represents the events for the `Member` entity
    - [`TechnicalReservationEvent.cs`](./Domain/Events/Reservation/TechnicalReservationEvent.cs)(_Line 1:8_): Represents the events for the `Reservation` entity
    - [`TechnicalCourtEvent.cs`](./Domain/Events/Court/TechnicalCourtEvent.cs)(_Line 1:7_): Represents the events for the `Court` entity
    - [`TechnicalClubEvent.cs`](./Domain/Events/Club/TechnicalClubEvent.cs)(_Line 1:7_): Represents the events for the `Club` entity
- [`BaseEvent.cs`](./Domain/Events/BaseEvent.cs)(_Line 1:34_): Represents the whole event including the following metadata:
Each event class represents a specific type of event that can occur in the system.
  - `event_id`: The unique identifier for the event
  - `entity_id`: The unique identifier for the entity that the event belongs to
  - `event_type`: The type of the event
  - `entity_type`: The type of the entity that the event belongs to
  - `timestamp`: The timestamp when the event occurred
  - `correlation_id`: The correlation id of the event
- [`DomainEvent.cs`](./Domain/Events/DomainEvent.cs)(_Line 1:35_): Is used as the data type of the `eventData` property in the `BaseEvent` class. It is also used for json serialization and deserialization.

The smallest unit of events can be found in the [Events](./Domain/Events) folder. Each event class represents a specific type of event that can occur in the system and implements the `DomainEvent` class.
- **PlayOfferEvents**:
  - [`PlayOfferCreatedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferCreatedEvent.cs)(_Line 1:26_): Represents the event when a PlayOffer is created
  - [`PlayOfferJoinedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferJoinedEvent.cs)(_Line 1:9_): Represents the event when a Opponent joins a PlayOffer
  - [`PlayOfferCancelledEvent.cs`](./Domain/Events/PlayOffer/PlayOfferCancelledEvent.cs)(_Line 1:6_): Represents the event when a PlayOffer is canceled
  - [`PlayOfferReservationAddedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferReservationAddedEvent.cs)(_Line 1:6_): Represents the event when a Reservation was created by the court service and was now added to the PlayOffer
  - [`PlayOfferOpponentRemovedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferOpponentRemovedEvent.cs)(_Line 1:5_): Represents the event when no Reservation could be created by the court service and therefore the opponent was removed from the PlayOffer


- **MemberEvents**:
  - [`MemberCreatedEvent.cs`](./Domain/Events/Member/MemberCreatedEvent.cs)(_Line 1:13_): Represents the event when a Member is created
  - [`MemberDeletedEvent.cs`](./Domain/Events/Member/MemberDeletedEvent.cs)(_Line 1:5_): Represents the event when a Member is deleted
  - [`MemberEmailChangedEvent.cs`](./Domain/Events/Member/MemberEmailChangedEvent.cs)(_Line 1:6_): Represents the event when the email of a Member is changed
  - [`MemberFullNameChangedEvent.cs`](./Domain/Events/Member/MemberFullNameChangedEvent.cs)(_Line 1:8_): Represents the event when the name of a Member is changed
  - [`MemberLockedEvent.cs`](./Domain/Events/Member/MemberLockedEvent.cs)(_Line 1:6_): Represents the event when a Member is locked
  - [`MemberUnlockedEvent.cs`](./Domain/Events/Member/MemberUnlockedEvent.cs)(_Line 1:6_): Represents the event when a Member is unlocked


- **ReservationEvents**:
  - [`ReservationCreatedEvent.cs`](./Domain/Events/Reservation/ReservationCreatedEvent.cs)(_Line 1:21_): Represents the event when a Reservation is created
  - [`ReservationCancelledEvent.cs`](./Domain/Events/Reservation/ReservationCancelledEvent.cs)(_Line 1:5_): Represents the event when a Reservation is canceled
  - [`ReservationLimitExceededEvent.cs`](./Domain/Events/Reservation/ReservationLimitExceededEvent.cs)(_Line 1:21_): Represents the event when the limit of Reservations per Member is exceeded
  - [`ReservationRejectedEvent.cs`](./Domain/Events/Reservation/ReservationRejectedEvent.cs)(_Line 1:6_): Represents the event when a Reservation could not be created


- **CourtEvents**:
    - [`CourtCreatedEvent.cs`](./Domain/Events/Court/CourtCreatedEvent.cs)(_Line 1:14_): Represents the event when a Court is created
    - [`CourtUpdatedEvent.cs`](./Domain/Events/Court/CourtUpdatedEvent.cs)(_Line 1:12_): Represents the event when a Court is changed


- **ClubEvents**:
    - [`ClubCreatedEvent.cs`](./Domain/Events/Club/ClubCreatedEvent.cs)(_Line 1:13_): Represents the event when a Club is created
    - [`ClubDeletedEvent.cs`](./Domain/Events/Club/ClubDeletedEvent.cs)(_Line 1:16_): Represents the event when a Club is deleted
    - [`ClubNameChangedEvent.cs`](./Domain/Events/Club/ClubNameChangedEvent.cs)(_Line 1:6_): Represents the event when the name of a Club is changed
    - [`ClubLockedEvent.cs`](./Domain/Events/Club/ClubLockedEvent.cs)(_Line 1:6_): Represents the event when a Club is locked
    - [`ClubUnlockedEvent.cs`](./Domain/Events/Club/ClubUnlockedEvent.cs)(_Line 1:6_): Represents the event when a Club is unlocked


The events are applied to the entities in the `apply` methods, the implementation location can be found under [Domain Driven Design](#domain-driven-design).

#### Idempotent Events
In the PlayOfferService, the idempotency of all events is guaranteed!

All events which were read from the redis stream and were processed by the `EventHandlers` are saved into the `AppliedEvents` table in the read side database. This allows us to check if a received event was already processed and therefore can be ignored.
Therefore the outcome of all events won't change if they are processed multiple times.

### Authentication and Authorization
In the PlayOfferService, Authentication and Authorization are implemented using a JWT token, which is is provided by the club service. All requests to the PlayOfferService must include a valid JWT token in the Authorization header.

All Queries can be executed by users with the `ADMIN` and `MEMBER` role. The commands can only be executed by users with the `MEMBER` roles.
A custom [`JwtClaimsMiddleware.cs`](./JwtClaimsMiddleware.cs)(_Line 1:43_) is used to extract the claims from the JWT token and add them to the `HttpContext` of the request.

These claims are then checked with the `Authorize` attribute in the [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Lines 31,55,80,115,147,181_) to ensure that the user has the necessary roles to execute the request.
Furthermore, most requests also extract the `memberId` and/or the `clubId` from the claims to ensure that the user can only access their own data, this can be seen in [`PlayOfferController.cs`](./Application/Controllers/PlayOfferController.cs)(_Lines 39,63,122:123,154,189_).

### Optimistic Locking
In the PlayOfferService, Optimistic Locking is implemented using the `EFCore` and its transaction mechanism. When a request is received, the current amount of events is read and incremented by one.

When the request is processed, the amount of events is read again and compared to the initial amount. If the amount of events has changed unexpectedly during the transaction, a concurrency exception is thrown and the transaction rolled back.

Otherwise the transaction is committed and the changes are saved to the database.

The Optimistic Locking is implemented in the each CommandHandler in the [Commands](./Application/Handlers) folder.

- [`CancelPlayOfferHandler.cs`](./Application/Handlers/CancelPlayOfferHandler.cs)
  - _Line 26:27_
  - _Line 67:75_
- [`JoinPlayOfferHandler.cs`](./Application/Handlers/JoinPlayOfferHandler.cs)
  - _Line 29:30_
  - _Line 88:96_
- [`CreatePlayOfferHandler.cs`](./Application/Handlers/CreatePlayOfferHandler.cs)
  - _Line 69:70_
  - _Line 75:83_

### Domain Driven Design
In the PlayOfferService, DDD is used to model the core domain of the application, which includes the following entities:

- [`PlayOffer.cs`](./Domain/Models/PlayOffer.cs)(_Line 1:81_): Represents a play offer that is created by a member and can be joined by other members
- [`Member.cs`](./Domain/Models/Member.cs)(_Line 1:81_): Represents a member of the platform who can create and join play offers
- [`Reservation.cs`](./Domain/Models/Reservation.cs)(_Line 1:51_): Represents a reservation for a play offer that is created by the court service
- [`Court.cs`](./Domain/Models/Court.cs)(_Line 1:45_): Represents a court that can be reserved for a play offer
- [`Club.cs`](./Domain/Models/Club.cs)(_Line 1:66_): Represents a club that can have multiple courts and members

Since event sourcing was also used each entity implements a `apply` method which is used to apply the events to the entity. It is important to note that the `apply` method is not allowed to fail, as it is used to reconstruct the state of the entity and the correctness of the events is guaranteed by the `CommandHandlers
`.
The implementation for the `apply` methods can be found here:
- [`PlayOffer.cs`](./Domain/Models/PlayOffer.cs)(_Line 23_)
- [`Member.cs`](./Domain/Models/Member.cs)(_Line 17_)
- [`Reservation.cs`](./Domain/Models/Reservation.cs)(_Line 18_)
- [`Court.cs`](./Domain/Models/Court.cs)(_Line 14_)
- [`Club.cs`](./Domain/Models/Club.cs)(_Line 14_)

However, we didn't implement a `process` method in each entity, since the processing of the events is done in the `CommandHandlers`.

### Transaction Log Trailing
In the PlayOfferService, Transaction Log Trailing is implemented using Debezium, which is an open-source platform for change data capture. Debezium captures changes to the PostgreSQL database and publishes them to a Redis Stream.

The Debezium configuration can be found in the [pos_debezium.yml](./debezium-conf/application.properties)(_Line 1:21_) file.
