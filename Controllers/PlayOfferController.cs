using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlayOfferService.Commands;
using PlayOfferService.Models;
using PlayOfferService.Queries;
using PlayOfferService.Repositories;

namespace PlayOfferService.Controllers;

[ApiController]
[Route("api")]
public class PlayOfferController : ControllerBase
{

    private readonly DatabaseContext _context;
    private readonly IMediator _mediator;

    public PlayOfferController(DatabaseContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    ///<summary>
    ///Retrieve all Play Offers matching the query params
    ///</summary>
    ///<param name="playOfferId">The id of the play offer</param>
    ///<param name="creatorId">The id of the creator of the play offer</param>
    ///<param name="clubId">The id of the club of the play offer</param>
    ///<returns>Play offer with a matching id</returns>
    ///<response code="200">Returns a Play offer matching the query params</response>
    ///<response code="204">No Play offer with matching properties was found</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOffer>>> GetByIdAsync([FromQuery] Guid? playOfferId, [FromQuery] Guid? creatorId, [FromQuery] Guid? clubId)
    {

        var result = await _mediator.Send(new GetPlayOffersByIdQuery(playOfferId, creatorId, clubId));

        if (result.Count() == 0) return NoContent();

        return Ok(result);
    }


    ///<summary>
    ///Create a new Play Offer
    ///</summary>
    ///<param name="playOfferDto">The Play Offer to create</param>
    ///<returns>The newly created Play offer</returns>
    ///<response code="200">Returns the newly created Play offer</response>
    ///<response code="400">Invalid Play Offer structure</response>
    [HttpPost]
    [ProducesResponseType(typeof(PlayOffer), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<PlayOffer>> Create(PlayOfferDto playOfferDto)
    {
        // TODO: Check if creatorId is valid, and retrieve clubId
        var result = await _mediator.Send(new CreatePlayOfferCommand(playOfferDto));

        return CreatedAtAction(nameof(GetByIdAsync), new { playOfferId = result.Id }, result);
    }

    ///<summary>
    ///Deletes a Play Offer with a matching id
    ///</summary>
    ///<param name="playOfferId">The id of the Play Offer to delete</param>
    ///<returns>Nothing</returns>
    ///<response code="200">The Play Offer with the matching id was deleted</response>
    ///<response code="400">No Play Offer with matching id found</response>
    [HttpDelete]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult> Delete(Guid playOfferId)
    {
        var result = _mediator.Send(new DeletePlayOfferCommand(playOfferId));

        if (result.Exception != null) return BadRequest();

        return Ok();
    }

    ///<summary>
    ///Adds a given opponentId to a Play Offer and creates a reservation
    ///</summary>
    ///<param name="joinPlayOfferDto">The opponentId to add to the Play Offer with the matching playOfferId</param>
    ///<returns>Nothing</returns>
    ///<response code="200">The opponentId was added to the Play Offer with the matching playOfferId</response>
    ///<response code="400">No playOffer with a matching playOfferId found</response>
    [HttpPost]
    [Route("/join")]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult Join(JoinPlayOfferDto joinPlayOfferDto)
    {
        var playOffer = _context.PlayOffers.FirstOrDefault(po => po.Id == joinPlayOfferDto.PlayOfferId);

        // TODO: Check if opponentId is valid, and retrieve clubId
        if (playOffer == null) return BadRequest();
        playOffer.Opponent = new Member { Id = joinPlayOfferDto.OpponentId };
        _context.SaveChanges();

        //TODO: Send request to reservation service to create a reservation and update playOffer.ReservationId
        return Ok();
    }
}