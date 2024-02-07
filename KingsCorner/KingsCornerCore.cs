/*
Copyright 2024 Cekay3D

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Cekay.KingsCorner
{
    public class KingsCornerCore : UdonSharpBehaviour
    {
        public AudioSource CardAudio;
        public AudioClip Grab;
        public AudioClip Drop;
        public string[] Players;
        public KingsCornerStack[] OrderedStacks;
        public KingsCornerCard[] OrderedCards;
        public KingsCornerCard[] P1Cards;
        public KingsCornerCard[] P2Cards;
        public KingsCornerCard[] P3Cards;
        public KingsCornerCard[] P4Cards;
        public KingsCornerCard[] DrawPileCards;
        public KingsCornerCard[] NPileCards;
        public KingsCornerCard[] SPileCards;
        public KingsCornerCard[] EPileCards;
        public KingsCornerCard[] WPileCards;
        public KingsCornerCard[] NWPileCards;
        public KingsCornerCard[] NEPileCards;
        public KingsCornerCard[] SEPileCards;
        public KingsCornerCard[] SWPileCards;

        private bool IsColliding;
        private bool IsGraviational;
        private bool InProgress = true;
        private bool P1Active;
        private bool P2Active;
        private bool P3Active;
        private bool P4Active;
        private bool Hints = true;
        private int CurrentPlayerTurn = 0;
        private VRCPlayerApi LocalPlayer;
        private string WinnerName;

        [SerializeField] private VRCObjectPool CardPool;
        [SerializeField] private TextMeshProUGUI P1Name;
        [SerializeField] private TextMeshProUGUI P2Name;
        [SerializeField] private TextMeshProUGUI P3Name;
        [SerializeField] private TextMeshProUGUI P4Name;
        [SerializeField] private TextMeshProUGUI WinnerText;
        [SerializeField] private GameObject StartCanvas;
        [SerializeField] private GameObject HowToCanvas;
        [SerializeField] private GameObject AboutCanvas;
        [SerializeField] private GameObject WinCanvas;
        [SerializeField] private GameObject DrawPile;
        [SerializeField] private GameObject P1Join;
        [SerializeField] private GameObject P2Join;
        [SerializeField] private GameObject P3Join;
        [SerializeField] private GameObject P4Join;
        [SerializeField] private GameObject P1Display;
        [SerializeField] private GameObject P2Display;
        [SerializeField] private GameObject P3Display;
        [SerializeField] private GameObject P4Display;
        [SerializeField] private GameObject P1TurnDisplay;
        [SerializeField] private GameObject P2TurnDisplay;
        [SerializeField] private GameObject P3TurnDisplay;
        [SerializeField] private GameObject P4TurnDisplay;
        [SerializeField] private GameObject[] P1Layout;
        [SerializeField] private GameObject[] P2Layout;
        [SerializeField] private GameObject[] P3Layout;
        [SerializeField] private GameObject[] P4Layout;
        [SerializeField] private GameObject[] StartStacksLayout;
        [SerializeField] private GameObject[] NPileLayout;
        [SerializeField] private GameObject[] SPileLayout;
        [SerializeField] private GameObject[] EPileLayout;
        [SerializeField] private GameObject[] WPileLayout;
        [SerializeField] private GameObject[] NWPileLayout;
        [SerializeField] private GameObject[] NEPileLayout;
        [SerializeField] private GameObject[] SEPileLayout;
        [SerializeField] private GameObject[] SWPileLayout;

        void Start()
        {
            LocalPlayer = Networking.LocalPlayer;
            Players = new string[] { "Player", "Player", "Player", "Player" };

            foreach (KingsCornerCard k in OrderedCards)
            {
                //IsColliding = false;
                //IsGraviational = false;

                Collider c = k.GetComponent<Collider>();
                c.isTrigger = true;
            }
        }

        public void SetPlayer1()
        {
            Players[0] = LocalPlayer.displayName;
            P1Join.SetActive(false);
            P1Name.text = Players[0];
            P1Display.SetActive(true);
            P1Active = true;
        }

        public void SetPlayer2()
        {
            Players[1] = LocalPlayer.displayName;
            P2Join.SetActive(false);
            P2Name.text = Players[1];
            P2Display.SetActive(true);
            P2Active = true;
        }

        public void SetPlayer3()
        {
            Players[2] = LocalPlayer.displayName;
            P3Join.SetActive(false);
            P3Name.text = Players[2];
            P3Display.SetActive(true);
            P3Active = true;
        }

        public void SetPlayer4()
        {
            Players[3] = LocalPlayer.displayName;
            P4Join.SetActive(false);
            P4Name.text = Players[3];
            P4Display.SetActive(true);
            P4Active = true;
        }

        public void GlobalStartGame()
        {
            if (Players.Length != 0)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(StartGame));
            }
        }

        public void SetHints()
        {

        }

        public void StartGame()
        {
            P1Join.SetActive(false);
            P2Join.SetActive(false);
            P3Join.SetActive(false);
            P4Join.SetActive(false);

            InProgress = true;
            StartCanvas.SetActive(false);
            WinCanvas.SetActive(false);

            CardPool.Shuffle();

            if (P1Active)
            {
                DealHand(P1Layout, 1);
            }
            if (P2Active)
            {
                DealHand(P2Layout, 2);
            }
            if (P3Active)
            {
                DealHand(P3Layout, 3);
            }
            if (P4Active)
            {
                DealHand(P4Layout, 4);
            }

            DealStack(StartStacksLayout[0], 5);
            DealStack(StartStacksLayout[1], 6);
            DealStack(StartStacksLayout[2], 7);
            DealStack(StartStacksLayout[3], 8);
        }

        public void ShowHowTo()
        {
            HowToCanvas.SetActive(!HowToCanvas.activeSelf);
        }
        
        public void ShowAbout()
        {
            AboutCanvas.SetActive(!AboutCanvas.activeSelf);
        }

        public void DealHand(GameObject[] layout, int location)
        {
            foreach (GameObject g in layout)
            {
                GameObject drawn = CardPool.TryToSpawn();
                drawn.transform.position = g.transform.position;
                drawn.transform.rotation = g.transform.rotation;

                KingsCornerCard card = drawn.GetComponent<KingsCornerCard>();
                int tempIndex = card.DeckIndex;
                OrderedCards[tempIndex].Owner = location;
                OrderedCards[tempIndex].ReturnSpot = g;
            }
        }

        public void DealStack(GameObject g, int location)
        {
            GameObject drawn = CardPool.TryToSpawn();
            drawn.transform.position = g.transform.position;
            drawn.transform.rotation = g.transform.rotation;

            KingsCornerCard card = drawn.GetComponent<KingsCornerCard>();
            int tempIndex = card.DeckIndex;
            OrderedCards[tempIndex].Owner = location;
            OrderedCards[tempIndex].ReturnSpot = g;

            KingsCornerStack stack = g.GetComponentInParent<KingsCornerStack>();
            int stackIndex = stack.StackID;
            OrderedStacks[stackIndex].BottomCard = card.DeckIndex;
            OrderedStacks[stackIndex].TopCard = card.DeckIndex;
            OrderedStacks[stackIndex].TotalCards = 1;
        }

        public void PlayCard()
        {

        }

        public void CheckWin()
        {
            if (P1Cards.Length == 0 ||
                P2Cards.Length == 0 ||
                P3Cards.Length == 0 ||
                P4Cards.Length == 0)
            {
                WinGame();
            }
        }

        public void NextPlayer()
        {
            CurrentPlayerTurn++;

            if (Players[CurrentPlayerTurn] == null)
            {
                NextPlayer();
            }
        }

        public void WinGame()
        {
            InProgress = false;
            StartCanvas.SetActive(true);
            WinCanvas.SetActive(true);
            WinnerText.text = WinnerName;
        }
    }
}