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

using UdonSharp;
using UnityEngine;

namespace Cekay.KingsCorner
{
    public class KingsCornerCard : UdonSharpBehaviour
    {
        public int Color; // 0 = Black, 1 = Red
        public int Value;
        public int Suit; // 0 = Spades, 1 = Hearts, 2 = Clubs, 3 = Diamonds
        public int DeckIndex; // from 0-51
        public int Grabbable;
        public int Owner; // Where is the card? Corresponds to StackID: 1-4 is players, 0 is in deck, 5-8 is on stack, 9-12 is on corner
        public GameObject ReturnSpot;
        public GameObject NewSpot;
        public GameObject Deck;
        
        [SerializeField] private KingsCornerCore Core;

        private bool CanPlace = false;

        private void Start()
        {
        }

        public override void OnPickup()
        {
            Core.CardAudio.gameObject.transform.position = this.gameObject.transform.position;
            Core.CardAudio.PlayOneShot(Core.Grab);
        }

        public override void OnDrop()
        {
            if (ReturnSpot != null)
            {
                this.gameObject.transform.position = ReturnSpot.transform.position;
                this.gameObject.transform.rotation = ReturnSpot.transform.rotation;
            }
            Core.CardAudio.gameObject.transform.position = this.gameObject.transform.position;
            Core.CardAudio.PlayOneShot(Core.Drop);

            if (CanPlace)
            {
                ReturnSpot = NewSpot;
                this.gameObject.transform.position = ReturnSpot.transform.position;
                this.gameObject.transform.rotation = ReturnSpot.transform.rotation;
            }
        }

        public void OnTriggerEnter(Collider Other)
        {
            if (Other.gameObject.layer == 22)
            {
                KingsCornerStack stack = Other.GetComponentInParent<KingsCornerStack>();

                if (stack != null)
                {
                    KingsCornerCard currentCard = Core.OrderedCards[stack.TopCard];

                    if (Value == (currentCard.Value - 1) && Suit != currentCard.Suit)
                    {
                        CanPlace = true;
                        NewSpot = Other.gameObject;

                        int stackIndex = stack.StackID;
                        Core.OrderedStacks[stackIndex].TotalCards++;
                        Core.OrderedStacks[stackIndex].TopCard = currentCard.DeckIndex;
                    }
                }
            }

            Debug.Log(CanPlace.ToString());
        }

        public void OnTriggerExit(Collider Other)
        {
            if (Other.gameObject.layer == 22)
            {
                CanPlace = false;
            }
        }

        public void OnGameReset()
        {
            Owner = 0;
        }
    }
}