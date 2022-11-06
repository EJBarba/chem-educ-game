using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DaiMangou.BridgedData
{
    /// <summary>
    /// Character Node Data Representation (this class structure will greatly change in future updates to support Histry and Evolution like storyteller does)
    /// </summary>
    [Serializable]
    public class CharacterNodeData:NodeData
    {


        #region Personality traits

        #region positive traits
        /// <summary>
        ///  Collection of int values representing positive traits
        /// </summary>
        [Serializable]
        public class PositiveTraits
        {
            public float Accessible;
            public float Active;
            public float Adaptable;
            public float Adventurous;
            public float Alert;
            public float Brilliant;
            public float Calm;
            public float Capable;
            public float Challenging;
            public float Charismatic;
            public float Clever;
            public float Confident;
            public float Cooperative;
            public float Courageous;
            public float Curious;
            public float Daring;
            public float Decisive;
            public float Discreet;
            public float Efficient;
            public float Energetic;
            public float Focused;
            public float Helpful;
            public float Independent;
            public float Individualistic;
            public float Innovative;
            public float Intelligent;
            public float Leaderly;
            public float Logical;
            public float Loyal;
            public float Methodical;
            public float Orderly;
            public float Organized;
            public float Patient;
            public float Patriotic;
            public float Peaceful;
            public float Reliable;
            public float Resourceful;
            public float Responsible;
            public float Responsive;
            public float Sane;
            public float Selfless;
            public float Serious;
            public float Skillful;
            public float Stoic;
            public float Strong;
        }

        #endregion

        #region neutral traits
        /// <summary>
        /// Collection of int values representing neutral traits
        /// </summary>
        [Serializable]
        public class NeutralTraits
        {
            public float Aggressive;
            public float Deceptive;
            public float Dominating;
            public float Enigmatic;
            public float Experimental;
            public float Iconoclastic;
            public float Idiosyncratic;
            public float Impassive;
            public float Obedient;
            public float Progressive;
            public float Questioning;
            public float Reserved;
            public float Stubborn;
        }

        #endregion

        #region negative traits
        /// <summary>
        /// Collection of int values representing negative traits
        /// </summary>
        [Serializable]
        public class NegativeTraits
        {
            public float Angry;
            public float Arrogantt;
            public float Careless;
            public float Childish;
            public float Cowardly;
            public float Crafty;
            public float Crazy;
            public float Cruel;
            public float Dependent;
            public float Destructive;
            public float Devious;
            public float Disloyal;
            public float Disobedient;
            public float Disorderly;
            public float Disorganized;
            public float Fearful;
            public float Fickle;
            public float Foolish;
            public float Forgetful;
            public float Frightening;
            public float Frivolous;
            public float Graceless;
            public float Gullible;
            public float Hostile;
            public float Impractical;
            public float Irritable;
            public float Lazy;
            public float Meddlesome;
            public float Miserable;
            public float Neglectful;
            public float Opportunistic;
            public float Provocative;
            public float Slow;
            public float Timid;
            public float Uncooperative;
            public float Unreliable;
            public float Weak;
            public float WeakWilled;
        }

        #endregion

        #endregion

        #region Personality
        /// <summary>
        ///  Three sets of values used to help define your characters personality.
        /// </summary>
        [Serializable]
        public class Personality
        {
            public NegativeTraits NegativeTraits = new NegativeTraits();

            public NeutralTraits NeutralTraits = new NeutralTraits();

            public PositiveTraits PositiveTraits = new PositiveTraits();
        }
        #endregion
        /// <summary>
        /// characters have a list of all their actions , dialogue and other node data as shown in storyteller
        /// </summary>
        public List<NodeData> NodeDataInMyChain = new List<NodeData>();
        /// <summary>
        /// copty of the character image as seen in Storyteller 
        /// </summary>
        public Sprite CharacterImage;
        /// <summary>
        /// 
        /// </summary>
        public Personality CharacterPersonality = new Personality();
        /// <summary>
        /// the position in the Character listwhich this Node Data will exist 
        /// </summary>
        public int SortingID;

        public override void OnEnable()
        {
            type = GetType();
            base.OnEnable();
        }
    }
}
