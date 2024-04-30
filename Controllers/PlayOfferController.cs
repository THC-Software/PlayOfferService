using Microsoft.AspNetCore.Mvc;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Controllers;

[ApiController]
[Route("api")]
public class PlayOfferController: ControllerBase
{
    private DatabaseContext _context;
    
    public PlayOfferController(DatabaseContext context)
    {
        _context = context;
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
    public ActionResult<IEnumerable<PlayOffer>> GetById([FromQuery]Guid? playOfferId, [FromQuery]Guid? creatorId, [FromQuery] Guid? clubId)
    {
        var result = _context.PlayOffers.Where(po => po != null
                                                     && (!playOfferId.HasValue || po.Id == playOfferId)
                                                     && (!creatorId.HasValue || po.Creator.Id == creatorId)
                                                     && (!clubId.HasValue || po.Club.Id == clubId)
        ).ToList();
        return result.Count > 0 ? Ok(result) : NoContent();
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
    public ActionResult<PlayOffer> Create(PlayOfferDto playOfferDto)
    {
        // TODO: Check if creatorId is valid, and retrieve clubId
        var result = _context.PlayOffers.Add(new PlayOffer(playOfferDto));
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { playOfferId = result.Entity.Id }, result.Entity);
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
    public ActionResult Delete(Guid playOfferId)
    {
        var playOffer = _context.PlayOffers.FirstOrDefault(po => po.Id == playOfferId);
        if (playOffer == null) return BadRequest();
        _context.PlayOffers.Remove(playOffer);
        _context.SaveChanges();
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