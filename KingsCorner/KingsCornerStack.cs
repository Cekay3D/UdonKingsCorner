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
    public class KingsCornerStack : UdonSharpBehaviour
    {
        public int StackID;
        public int TopCard;
        public int BottomCard;
        public int TotalCards;

        [SerializeField] private KingsCornerCore Core;
        [SerializeField] private GameObject[] Slots;

        private void Start()
        {
            foreach (GameObject g in Slots)
            {
                //Collider col = g.GetComponent<Collider>();

                if (StackID == 5 ||
                    StackID == 6 ||
                    StackID == 7 ||
                    StackID == 8)
                {
                    if (g.name != "1") // Enable the second slot for stacks, since a first card will be dealt at start
                    {
                        g.SetActive(false);
                    }
                }

                if (StackID == 9 ||
                    StackID == 10 ||
                    StackID == 11 ||
                    StackID == 12)
                {
                    if (g.name != "0") // Enable the first slot for corners since they're empty at start
                    {
                        g.SetActive(false);
                    }
                }
            }
        }

        public void AddCard()
        {
            Collider oldCol = Slots[TotalCards].GetComponent<Collider>();
            oldCol.enabled = false;

            TotalCards++;

            Collider newCol = Slots[TotalCards].GetComponent<Collider>();
            newCol.enabled = true;
        }

        public void RemoveCard()
        {
            Collider oldCol = Slots[TotalCards].GetComponent<Collider>();
            oldCol.enabled = false;

            TotalCards--;

            Collider newCol = Slots[TotalCards].GetComponent<Collider>();
            newCol.enabled = true;
        }
    }
}