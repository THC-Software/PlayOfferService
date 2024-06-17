using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlayOfferService.Application.Commands;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Controllers;

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
    ///Retrieve all Play Offers of a club with a matching id
    ///</summary>
    ///<param name="clubId">The id of the club of the play offer</param>
    ///<returns>Play offers with a matching club id</returns>
    ///<response code="200">Returns a list of Play offers matching the query params</response>
    ///<response code="204">No Play offer with matching properties was found</response>
    [HttpGet]
    [Route("club")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOffer>>> GetByClubIdAsync([FromQuery] Guid clubId)
    {
        //TODO: refactor after jwt implementation to get clubId from token
        var result = await _mediator.Send(new GetPlayOffersByClubIdQuery(clubId));

        if (result.Count() == 0)
            return NoContent();

        return Ok(result);
    }
    
    ///<summary>
    ///Retrieve all Play Offers of a participating member
    ///</summary>
    ///<param name="participantId">The id of the member participating in the play offer</param>
    ///<returns>List of Play offers with where given member is creator or opponent</returns>
    ///<response code="200">Returns a list of Play offers matching the query params</response>
    ///<response code="204">No Play offer with matching properties was found</response>
    [HttpGet]
    [Route("participant")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOffer>>> GetByParticipantIdAsync([FromQuery] Guid participantId)
    {
        //TODO: refactor after jwt implementation to get participantId from token
        var result = await _mediator.Send(new GetPlayOffersByParticipantIdQuery(participantId));

        if (result.Count() == 0)
            return NoContent();

        return Ok(result);
    }
    
    ///<summary>
    ///Get all Play offers created by a member with a matching name
    ///</summary>
    ///<param name="creatorName">Name of the creator in the format '[FirstName] [LastName]', '[FirstName]' or '[LastName]'</param>
    ///<returns>A list of Play offers with a matching id</returns>
    ///<response code="200">Returns a List of Play offers with creator matching the query params</response>
    ///<response code="204">No Play offers with matching creator was found</response>
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOffer>>> GetByCreatorNameAsync([FromQuery] string creatorName)
    {
        IEnumerable<PlayOffer> result;
        try
        {
            result = await _mediator.Send(new GetPlayOffersByCreatorNameQuery(creatorName));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        if (result.Count() == 0)
            return NoContent();

        return Ok(result);
    }
    


    ///<summary>
    ///Create a new Play Offer
    ///</summary>
    ///<param name="createPlayOfferDto">The Play Offer to create</param>
    ///<returns>The newly created Play offer</returns>
    ///<response code="200">Returns the id of the created Play Offer</response>
    ///<response code="400">Invalid Play Offer structure</response>
    [HttpPost]
    [ProducesResponseType(typeof(PlayOffer), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<PlayOffer>> Create(CreatePlayOfferDto createPlayOfferDto)
    {
        Guid result;
        try
        {
            result = await _mediator.Send(new CreatePlayOfferCommand(createPlayOfferDto));
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
    [Route("join")]
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