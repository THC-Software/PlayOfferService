using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayOfferService.Application.Commands;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Controllers;

[ApiController]
[Route("api/playoffers")]
public class PlayOfferController : ControllerBase
{
    
    private readonly IMediator _mediator;

    public PlayOfferController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieve all Play Offers of the logged in users club
    /// </summary>
    /// <returns>Play offers with a matching club id</returns>
    /// <response code="200">Returns a list of Play offers matching the query params</response>
    /// <response code="204">No Play offer with matching properties was found</response>
    [HttpGet]
    [Authorize]
    [Route("club")]
    [ProducesResponseType(typeof(IEnumerable<PlayOfferDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOfferDto>>> GetByClubIdAsync()
    {
        var clubId = Guid.Parse(User.Claims.First(c => c.Type == "tennisClubId").Value);
        var result = await _mediator.Send(new GetPlayOffersByClubIdQuery(clubId));

        if (result.Count() == 0)
            return NoContent();

        return Ok(result);
    }
    
    ///<summary>
    ///Retrieve all Play Offers of a logged in user
    ///</summary>
    ///<returns>List of Play offers with where given member is creator or opponent</returns>
    ///<response code="200">Returns a list of Play offers matching the query params</response>
    ///<response code="204">No Play offer with matching properties was found</response>
    [HttpGet]
    [Authorize]
    [Route("participant")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOfferDto>>> GetByParticipantIdAsync()
    {
        var participantId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
    [Authorize]
    [Route("search")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<PlayOfferDto>>> GetByCreatorNameAsync([FromQuery] string creatorName)
    {
        IEnumerable<PlayOfferDto> result;
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
    ///Create a new Play Offer for the logged in user
    ///</summary>
    ///<param name="createPlayOfferDto">The Play Offer to create</param>
    ///<returns>The newly created Play offer</returns>
    ///<response code="200">Returns the id of the created Play Offer</response>
    ///<response code="400">Invalid Play Offer structure</response>
    ///<response code="401">Only members can create Play Offers</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PlayOffer), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<PlayOffer>> Create(CreatePlayOfferDto createPlayOfferDto)
    {
        if (User.Claims.First(c => c.Type == "groups").Value != "MEMBER")
            return Unauthorized("Only members can create Play Offers!");
        
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var clubId = Guid.Parse(User.FindFirst("tennisClubId")!.Value);
        
        Guid result;
        try
        {
            result = await _mediator.Send(new CreatePlayOfferCommand(createPlayOfferDto, creatorId, clubId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return CreatedAtAction(nameof(Create), new { playOfferId = result }, result);
    }

    ///<summary>
    ///Cancels a Play Offer with a matching id of the logged in user
    ///</summary>
    ///<param name="playOfferId">The id of the Play Offer to cancel</param>
    ///<returns>Nothing</returns>
    ///<response code="200">The Play Offer with the matching id was cancelled</response>
    ///<response code="400">No Play Offer with matching id found</response>
    ///<response code="401">Only creator can cancel Play Offers</response>
    [HttpDelete]
    [Authorize]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult> Delete(Guid playOfferId)
    {
        if (User.Claims.First(c => c.Type == "groups").Value != "MEMBER")
            return Unauthorized("Only members can cancel Play Offers!");
        
        var memberId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            await _mediator.Send(new CancelPlayOfferCommand(playOfferId, memberId));
        }
        catch (AuthorizationException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    ///<summary>
    ///Logged in user joins a Play Offer with a matching playOfferId
    ///</summary>
    ///<param name="joinPlayOfferDto">The opponentId to add to the Play Offer with the matching playOfferId</param>
    ///<returns>Nothing</returns>
    ///<response code="200">The opponentId was added to the Play Offer with the matching playOfferId</response>
    ///<response code="400">No playOffer with a matching playOfferId found</response>
    ///<response code="401">Only members can join Play Offers</response>
    [HttpPost]
    [Authorize]
    [Route("join")]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult> Join(JoinPlayOfferDto joinPlayOfferDto)
    {
        if (User.Claims.First(c => c.Type == "groups").Value != "MEMBER")
            return Unauthorized("Only members can join Play Offers!");
        
        var memberId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _mediator.Send(new JoinPlayOfferCommand(joinPlayOfferDto, memberId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }
}