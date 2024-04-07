using Microsoft.AspNetCore.Mvc;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Controllers;

[ApiController]
[Route("api")]
public class PlayOfferController: ControllerBase
{
    private PlayOfferContext _context;
    
    public PlayOfferController(PlayOfferContext context)
    {
        _context = context;
    }

    ///<summary>
    ///Retrieves all play offers with a matching clubId
    ///</summary>
    ///<param name="clubId">The clubId to filter by</param>
    ///<returns>A list of Play offers with a matching clubId</returns>
    ///<response code="200">Returns a list of Play offers with a matching clubId</response>
    ///<response code="204">No Play offers with a matching clubId were found</response>
    [HttpGet]
    [Route("club/{clubId}")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<PlayOffer>> GetByClubId(int clubId)
    {
        var result = _context.PlayOffers.Where(po => po.ClubId == clubId).ToList();
        return result.Count > 0 ? Ok(result) : NoContent();
    }
    
    ///<summary>
    ///Retrieves all play offers with a matching creatorId
    ///</summary>
    ///<param name="creatorId">The creatorId to filter by</param>
    ///<returns>A list of Play offers with a matching creatorId</returns>
    ///<response code="200">Returns a list of Play offers with a matching creatorId</response>
    ///<response code="204">No Play offers with a matching creatorId were found</response>
    [HttpGet]
    [Route("creator/{creatorId}")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<PlayOffer>> GetByCreatorId(int creatorId)
    {
        var result = _context.PlayOffers.Where(po => po.CreatorId == creatorId).ToList();
        return result.Count > 0 ? Ok(result) : NoContent();
    }
    
    ///<summary>
    ///Retrieves the play offer with a matching id
    ///</summary>
    ///<param name="playOfferId">The id to filter by</param>
    ///<returns>Play offer with a matching id</returns>
    ///<response code="200">Returns a Play offer with a matching id</response>
    ///<response code="204">No Play offer with a matching id was found</response>
    [HttpGet]
    [Route("{playOfferId}")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<PlayOffer>> GetById(int playOfferId)
    {
        var result = _context.PlayOffers.Where(po => po.Id == playOfferId).ToList();
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
    public ActionResult Delete(int playOfferId)
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
    public ActionResult Join(JoinPlayOfferDto joinPlayOfferDto)
    {
        var playOffer = _context.PlayOffers.FirstOrDefault(po => po.Id == joinPlayOfferDto.PlayOfferId);
        
        // TODO: Check if opponentId is valid, and retrieve clubId
        if (playOffer == null) return BadRequest();
        playOffer.OpponentId = joinPlayOfferDto.OpponentId;
        _context.SaveChanges();
        
        //TODO: Send request to reservation service to create a reservation and update playOffer.ReservationId
        return Ok();
    }
}