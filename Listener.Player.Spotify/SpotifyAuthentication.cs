using System.Timers;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;

namespace Listener.Player.Spotify
{
    internal class SpotifyAuthentication
    {
        public delegate void SpotifyClientReadyEventHandler(SpotifyAuthentication sender, SpotifyWebAPI client);
        public event SpotifyClientReadyEventHandler ClientReady;
        private readonly AuthorizationCodeAuth _authenFactory;
        private readonly Timer _refreshTokenWorker;
        private string _refreshToken;
        private SpotifyWebAPI _client { get; set; }
        internal SpotifyAuthentication()
        {
            bool initialized = false;
            var client_id = "7b2f38e47869431caeda389929a1908e";
            var secret_id = "c3a86330ef844c16be6cb46d5e285a45";
            _authenFactory = new AuthorizationCodeAuth(
                                client_id,
                                secret_id,
                                "http://localhost:8800",
                                "http://localhost:8800",
                                Scope.UserReadCurrentlyPlaying |
                                Scope.UserModifyPlaybackState |
                                Scope.AppRemoteControl |
                                Scope.UserReadPlaybackState
                                );
            _authenFactory.AuthReceived += async (s, p) =>
            {
                var ath = (AuthorizationCodeAuth)s;
                ath.Stop();

                var token = await ath.ExchangeCode(p.Code);
                _refreshToken = token.RefreshToken;
                if (_client == null)
                {
                    _client = new SpotifyWebAPI()
                    {
                        AccessToken = token.AccessToken,
                        TokenType = "Bearer"
                    };
                }
                else
                {
                    _client.AccessToken = token.AccessToken;
                }
                if (!initialized)
                    ClientReady?.Invoke(this, _client);
                initialized = true;

            };
            _authenFactory.Start();
            _authenFactory.OpenBrowser();
            _refreshTokenWorker = new Timer();
            _refreshTokenWorker.Interval = 30 * (1000 * 60);
            _refreshTokenWorker.Elapsed += async (s, e) =>
            {
                var token = await _authenFactory.RefreshToken(_refreshToken);
                _client.AccessToken = token.AccessToken;
            };
            _refreshTokenWorker.Start();
        }
    }
}
