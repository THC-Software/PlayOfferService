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
    
    [HttpGet]
    [Route("club/{clubId}")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<PlayOffer>> GetByClubId(int clubId)
    {
        var result = _context.PlayOffers.Where(po => po.ClubId == clubId).ToList();
        return result.Count > 0 ? Ok(result) : NoContent();
    }
    
    [HttpGet]
    [Route("creator/{creatorId}")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<PlayOffer>> GetByCreatorId(int creatorId)
    {
        var result = _context.PlayOffers.Where(po => po.CreatorId == creatorId).ToList();
        return result.Count > 0 ? Ok(result) : NoContent();
    }
    
    [HttpGet]
    [Route("{playOfferId}")]
    [ProducesResponseType(typeof(IEnumerable<PlayOffer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
    public ActionResult<IEnumerable<PlayOffer>> GetById(int playOfferId)
    {
        var result = _context.PlayOffers.Where(po => po.Id == playOfferId).ToList();
        return result.Count > 0 ? Ok(result) : NoContent();
    }
    
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