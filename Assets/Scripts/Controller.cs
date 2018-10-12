using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TicTacToe.Network;
using TicTacToe.UI;
using TicTacToe.Core;
using System;

namespace TicTacToe
{
    public class Controller : MonoBehaviour
    {
        [Header("Network")]
        [SerializeField] private GameServer server;
        [SerializeField] private GameClient client;

        [Header("UI")]
        [SerializeField] private BoardUI gameBoard;
        [SerializeField] private Text scoreText;
        [Space]
        [SerializeField] private GameObject networkDialog;
        [SerializeField] private Button serverButton;
        [SerializeField] private Button clientButton;
        [Space]
        [SerializeField] private GameObject roundDialog;
        [SerializeField] private Button startRoundButton;
        [SerializeField] private Button exitButton;
        [Space]
        [SerializeField] private Text infoText;

        private CellSign playerRoundSign;

        private void Start()
        {
            //Buttons
            serverButton.onClick.AddListener(OnServerClick);
            clientButton.onClick.AddListener(OnClientClick);
            startRoundButton.onClick.AddListener(OnStartRoundClick);
            exitButton.onClick.AddListener(OnExitClick);

            //Network
            client.OnNewRoundEvent += ClientNewRound;
            client.OnTurnEvent += ClientTurn;
            client.OnEndRoundEvent += ClientEndRoundEvent;
            client.OnDisconnect += Disconnect;

            server.OnDisconnect += Disconnect;           

            //Table
            gameBoard.OnClick += GameBoardClick;

            HideAll();
            ShowNetworkDialog();
        }


        private void Disconnect(UnityEngine.Networking.NetworkMessage netMsg)
        {
            ShowInfoText("Disconnect");
            OnExitClick();
        }

        private void GameBoardClick(object sender, CellClickArgs e)
        {
            if (!gameBoard.IsMarked(e.index))
            {
                client.SendTurnMesage(e.index, playerRoundSign);
            }
        }

        private void ClientNewRound(StartNewRoundMessage msg)
        {
            playerRoundSign = msg.yourSign;

            HideAll();
            ShowGameElements();

            ShowScoreText(msg.yourScore, msg.enemyScore);

            gameBoard.Lock = playerRoundSign != msg.whoseTurn;
            ShowInfoText(gameBoard.Lock ? "Enemy turn" : "Your turn");
        }

        private void ClientTurn(MakeTurnMessage msg)
        {
            gameBoard.MarkCell(msg.index, msg.whoseTurnWas);
            gameBoard.Lock = playerRoundSign == msg.whoseTurnWas;
            ShowInfoText(gameBoard.Lock ? "Enemy turn" : "Your turn");
        }

        private void ClientEndRoundEvent(EndRoundMessage msg)
        {
            HideAll();
            ShowScoreText(msg.yourScore, msg.enemyScore);
            ShowRoundDialog();
            gameBoard.Clear();

            string win = msg.winner == CellSign.None ? "Draw" :
                         msg.winner == playerRoundSign ? "You win" : "You lose";
            ShowInfoText(win);
        }

        private void OnServerClick()
        {
            if (!server.Listen())
            {
                ShowInfoText("Server error!");
            }
            else
            {
                client.Connect();

                HideAll();
                ShowRoundDialog();
            }
        }

        private void OnClientClick()
        {
            client.Connect();
            HideAll();
            ShowRoundDialog();
        }

        private void OnStartRoundClick()
        {
            client.SendReadyMessage();
            HideAll();
            ShowInfoText("Wait second player...");           
        }

        private void OnExitClick()
        {
            if (client.IsRuning) client.ShutDown();
            if (server.IsRuning) server.ShutDown();

            HideAll();
            ShowNetworkDialog();
        }

        private void HideAll()
        {
            gameBoard.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
            networkDialog.SetActive(false);
            roundDialog.SetActive(false);
            infoText.gameObject.SetActive(false);
        }

        private void ShowNetworkDialog()
        {
            networkDialog.SetActive(true);
        }

        private void ShowRoundDialog()
        {
            roundDialog.SetActive(true);
        }

        private void ShowInfoText(string msg)
        {
            infoText.gameObject.SetActive(true);
            infoText.text = msg;
        }

        private void ShowScoreText(int your, int enemy)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.text = string.Format("You:{0}\nEnemy:{1}", your, enemy);
        }

        private void ShowGameElements()
        {
            gameBoard.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
        }

    }
}
