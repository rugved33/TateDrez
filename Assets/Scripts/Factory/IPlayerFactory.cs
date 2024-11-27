using System.Collections.Generic;
using Game.Tatedrez.Model;

namespace Game.Tatedrez.Factory
{
    public interface IPlayerFactory
    {
        IPlayer CreatePiece(PlayerColor playerColor); 
    }
}
