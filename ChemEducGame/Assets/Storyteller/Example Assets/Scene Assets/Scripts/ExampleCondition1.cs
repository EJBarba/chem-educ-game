using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ExampleCondition1 : MonoBehaviour
    {

        public float health = 100;
        public bool glove;
        public bool canDrink;
        public bool _rich;








        public bool Rich
        {
            get
            {
                return _rich;
            }
            set
            {
                _rich = value;
            }
        }

        public bool Healthy
        {
            get
            {
                return health > 70;
            }


        }

        public bool HasLegendaryGloves
        {
            get
            {
                return glove;
            }
            set
            {
                glove = value;
            }
        }

        public bool AtLegalDrinkingAge
        {
            get
            {
                return canDrink;
            }
            set
            {
                canDrink = value;
            }

        }



        #region you can setup functions thisway too
        /* public bool Healthy()
         {
             return health > 70;
         }

         public bool HasLegendaryGloves()
         {
             return glove;
         }

         public bool AtLegalDrinkingAge()
         {
             return canDrink;
         }

         public bool HangarReady()
         {
             return CanGoIntohangar;
         }
         public bool AlreadyWentInhangar()
         {
             return WentInHangar;
         }
     */


        #endregion



    }