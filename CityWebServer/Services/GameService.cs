using System;
using ColossalFramework;

namespace CityWebServer.Services
{
    /// <summary>
    /// Gets game specific data from the game.
    /// </summary>
    internal class GameService
    {
        private readonly SimulationManager _simulationManager;

        /// <summary>
        /// Creates a new instance of the <see cref="GameService"/>.
        /// </summary>
        public GameService()
        {
            _simulationManager = Singleton<SimulationManager>.instance;
        }

        /// <summary>
        /// Gets the current time of the game.
        /// </summary>
        public DateTime GetGameTime()
        {
            return _simulationManager.m_currentGameTime.Date;
        }
    }
}
