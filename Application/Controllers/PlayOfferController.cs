using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlayOfferService.Commands;
using PlayOfferService.Models;
using PlayOfferService.Queries;

namespace PlayOfferService.Controllers;

[ApiController]
[Route("api")]
public class PlayOfferController : ControllerBase
{
    
    private readonly IMediator _mediator;

    public PlayOfferController(IMediator mediator)
    {
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

        if (result.Count() == 0)
            return NoContent();

        return Ok(result);
    }


    ///<summary>
    ///Create a new Play Offer
    ///</summary>
    ///<param name="playOfferDto">The Play Offer to create</param>
    ///<returns>The newly created Play offer</returns>
    ///<response code="200">Returns the id of the created Play Offer</response>
    ///<response code="400">Invalid Play Offer structure</response>
    [HttpPost]
    [ProducesResponseType(typeof(PlayOffer), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<PlayOffer>> Create(PlayOfferDto playOfferDto)
    {
        Guid result;
        try
        {
            result = await _mediator.Send(new CreatePlayOfferCommand(playOfferDto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return CreatedAtAction(nameof(Create), new { playOfferId = result }, result);
    }

    ///<summary>
    ///Cancels a Play Offer with a matching id
    ///</summary>
    ///<param name="playOfferId">The id of the Play Offer to cancel</param>
    ///<returns>Nothing</returns>
    ///<response code="200">The Play Offer with the matching id was cancelled</response>
    ///<response code="400">No Play Offer with matching id found</response>
    [HttpDelete]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult> Delete(Guid playOfferId)
    {
        try
        {
            await _mediator.Send(new CancelPlayOfferCommand(playOfferId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

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
    public async Task<ActionResult> Join(JoinPlayOfferDto joinPlayOfferDto)
    {
        try
        {
            await _mediator.Send(new JoinPlayOfferCommand(joinPlayOfferDto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }
}