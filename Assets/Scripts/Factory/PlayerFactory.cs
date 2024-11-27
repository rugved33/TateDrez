using System;
using Game.Tatedrez.Model;
using System.Collections.Generic;

namespace Game.Tatedrez.Factory
{
    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer CreatePiece(PlayerColor playerColor)
        {
            Player player = new Player(playerColor);
            return player;
        }
    }
}
