using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;

namespace OffenseDefense.Client
{
    class OffDefGame : BaseScript
    {
        /* -------------------------------------------------------------------------- */
        /*                                  Variables                                 */
        /* -------------------------------------------------------------------------- */
        private Vector3 runnerSpawn;
        private float runnerHeading;
        private Vector3 blockerSpawn;
        private float blockerHeading;

        private List<Shared.MapMarker> checkpoints;
        private bool[] completedCheckpoints;

        private Blip currentBlip;
        private int currentCheckpoint = -100;
        private int currentCheckpointIndex = -1;

        private string teamColor;
        private string role;

        private Vehicle myCar;

        private string runnerCar;
        private string blockerCar;

        // Countdown
        private const int timePerCountDown = 1000;
        private const int startingNumber = 5;
        private int currentCount = startingNumber;

        public bool gameActive = false;
        private bool gameOver = false;
        private string winningTeam = "";

        public bool checkActive = false;

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public OffDefGame()
        {
            // Events
            EventHandlers.Add("OffDef:CountdownTimer", new Action<int>(SendCountdownTimer));
            EventHandlers.Add("OffDef:EndGame", new Action<string>(EndGame));
            EventHandlers.Add("OffDef:UpdateScoreboard", new Action<dynamic, int>(UpdateScoreboard));

            Game.Player.CanControlCharacter = true;
        }

        /* -------------------------------------------------------------------------- */
        /*                               Vehicle Control                              */
        /* -------------------------------------------------------------------------- */
        private async Task<Vehicle> SpawnRunnerCar(string vehicle)
        {
            if (Util.IsPossibleCar(vehicle))
            {
                Util.RequestModel(vehicle);
                Vehicle car = await World.CreateVehicle(vehicle, this.runnerSpawn, this.runnerHeading);
                Util.SetCarLicensePlate(car, "RUNNER");

                return car;
            }
            else
            {
                return null;
            }
        }

        private async Task<Vehicle> SpawnBlockerCar(string vehicle)
        {
            if (Util.IsPossibleCar(vehicle))
            {
                Util.RequestModel(vehicle);
                Vehicle car = await World.CreateVehicle(vehicle, this.blockerSpawn, this.blockerHeading);
                Util.SetCarLicensePlate(car, "BLOCKER");

                return car;
            }
            else
            {
                return null;
            }
        }

        private async Task<Vehicle> SpawnCar()
        {
            Vehicle car;
            if (this.role == "Runner")
            {
                car = await SpawnRunnerCar(this.runnerCar);
            }
            else
            {
                car = await SpawnBlockerCar(this.blockerCar);
            }

            TeamColor carColor = TeamColors.list[this.teamColor];
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();

            this.myCar = car;
            return car;
        }

        public void DestroyCar()
        {
            this.myCar.Delete();
        }

        public void SetCarTypes(string runnerCar, string blockerCar)
        {
            if (runnerCar == "")
            {
                this.runnerCar = "Voodoo";
            }
            else
            {
                this.runnerCar = runnerCar;
            }

            if (blockerCar == "")
            {
                this.blockerCar = "Insurgent2";
            }
            else
            {
                this.blockerCar = blockerCar;
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                Spawn Control                               */
        /* -------------------------------------------------------------------------- */
        public void SetSpawn(Vector3 newRunnerSpawn, float newRunnerHeading, Vector3 newBlockerSpawn, float newBlockerHeading)
        {
            this.runnerSpawn = newRunnerSpawn;
            this.runnerHeading = newRunnerHeading;
            this.blockerSpawn = newBlockerSpawn;
            this.blockerHeading = newBlockerHeading;
        }

        public async void RespawnPlayer()
        {
            DestroyCar();
            await SpawnCar();
            PreparePlayer(false);
        }

        private void PreparePlayer(bool startingGame = true)
        {
            Ped player = Game.Player.Character;
            player.SetIntoVehicle(this.myCar, VehicleSeat.Driver);
            player.IsCollisionEnabled = true;
            player.IsVisible = true;

            if (startingGame)
            {
                Game.Player.CanControlCharacter = false;
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                 Player Info                                */
        /* -------------------------------------------------------------------------- */
        public void SetRole(string newRole)
        {
            this.role = newRole;
        }

        public string GetRole()
        {
            return this.role;
        }

        public void SetTeamColor(string newColor)
        {
            this.teamColor = newColor;
        }

        public string GetTeamColor()
        {
            return this.teamColor;
        }

        /* -------------------------------------------------------------------------- */
        /*                                 Checkpoints                                */
        /* -------------------------------------------------------------------------- */
        public void SetCheckpoints(List<Shared.MapMarker> checkpoints)
        {
            this.checkpoints = checkpoints;
            this.completedCheckpoints = new bool[checkpoints.Count];
            for (int i = 0; i < completedCheckpoints.Length; i++)
            {
                this.completedCheckpoints[i] = false;
            }
        }

        public async void CheckCheckpoints()
        {
            this.checkActive = true;
            if (role == "Runner")
            {
                Vector3 playerLoc = Game.Player.Character.Position;
                Vector3 cpLoc = checkpoints[currentCheckpointIndex].position;

                if (!this.completedCheckpoints[currentCheckpointIndex])
                {
                    if (Util.distanceBetweenPointsNoHeight(playerLoc, cpLoc) < 5)
                    {
                        this.completedCheckpoints[currentCheckpointIndex] = true;
                        CreateCheckpointAndBlip();

                        TriggerServerEvent("OffDef:AddTeamPoint", this.teamColor);
                    }
                }
            }
            await Delay(10);
            this.checkActive = false;
        }

        private void CreateCheckpointAndBlip()
        {
            if (currentBlip != null)
            {
                currentBlip.Delete();
            }

            if (currentCheckpoint != -100)
            {
                API.DeleteCheckpoint(currentCheckpoint);
            }

            if (this.gameActive)
            {
                for (int i = 0; i < this.completedCheckpoints.Length; i++)
                {
                    if (this.completedCheckpoints[i] == false)
                    {
                        currentCheckpointIndex = i;
                        break;
                    }
                }

                if (currentCheckpointIndex != -1)
                {
                    Vector3 currentCheckpointPos = this.checkpoints[currentCheckpointIndex].position;
                    if (currentCheckpointIndex == this.checkpoints.Count - 1)
                    {
                        currentCheckpoint = API.CreateCheckpoint(4, currentCheckpointPos.X, currentCheckpointPos.Y, currentCheckpointPos.Z, currentCheckpointPos.X, currentCheckpointPos.Y, currentCheckpointPos.Z, 8.0f, 255, 255, 0, 255, 0);
                    }
                    else
                    {
                        Vector3 nextCheckpointPos = this.checkpoints[currentCheckpointIndex + 1].position;
                        currentCheckpoint = API.CreateCheckpoint(0, currentCheckpointPos.X, currentCheckpointPos.Y, currentCheckpointPos.Z, nextCheckpointPos.X, nextCheckpointPos.Y, nextCheckpointPos.Z, 8.0f, 255, 255, 0, 255, 0);
                    }

                    currentBlip = World.CreateBlip(currentCheckpointPos);
                    currentBlip.Color = BlipColor.Yellow;
                    currentBlip.Sprite = BlipSprite.Standard;
                    currentBlip.ShowRoute = true;
                }
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                General Game                                */
        /* -------------------------------------------------------------------------- */
        public async void StartGame()
        {
            Util.SendChatMsg("Offense Defense is starting!", 0, 255, 255);
            await SpawnCar();
            PreparePlayer();
            this.gameActive = true;
            CreateCheckpointAndBlip();

            Debug.WriteLine("Client Ready!");
            TriggerServerEvent("OffDef:ClientReady", Game.Player.Name);
        }

        public void DisableControls()
        {
            Game.DisableControlThisFrame(1, Control.VehicleExit);
            Game.DisableControlThisFrame(1, Control.VehicleAttack);
        }

        private void PostCountdown()
        {
            Game.Player.CanControlCharacter = true;
        }

        public void EndGame(string winningTeam)
        {
            this.winningTeam = winningTeam;
            this.gameOver = true;

            this.gameActive = false;
            DrawEndGame();

            if (currentBlip != null)
            {
                currentBlip.Delete();
            }

            if (currentCheckpoint != -100)
            {
                API.DeleteCheckpoint(currentCheckpoint);
            }

            Game.Player.Character.Kill();
        }

        private void UpdateScoreboard(dynamic ranks, int totalCheckpoints)
        {
            Payload payload = new Payload();
            payload.scoreboardEnable = true;
            payload.scoreboardPayload = ranks;
            payload.scoreboardNeededPoints = totalCheckpoints;

            Util.SendNuiMessage(payload);
        }

        /* -------------------------------------------------------------------------- */
        /*                                     UI                                     */
        /* -------------------------------------------------------------------------- */
        private void DrawCountdown()
        {
            string outString = "";

            if (this.currentCount == 0)
            {
                outString = "GO!";
                Util.SendChatMsg(outString, 0, 255, 0);
            }
            else
            {
                outString = this.currentCount.ToString();
                Util.SendChatMsg(outString, 255, 255, 0);
            }

        }

        private void DrawEndGame()
        {
            if (this.gameOver)
            {
                string outString = $"Winner: {Util.FirstLetterToUpper(this.winningTeam)}";
                TeamColor winningColor = TeamColors.list[this.winningTeam];

                Util.SendChatMsg(outString, winningColor.r, winningColor.g, winningColor.b);
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                               Event Handlers                               */
        /* -------------------------------------------------------------------------- */
        private void SendCountdownTimer(int count)
        {
            this.currentCount = count;
            DrawCountdown();

            if (count == 0)
            {
                PostCountdown();
            }
        }
    }
}