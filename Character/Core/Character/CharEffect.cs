using System.Collections.Generic;

namespace Character.Core.Character
{
    public class CharEffect
    {
        public static readonly Dictionary<Id, string> Paths = new Dictionary<Id, string>()
        {
            [Id.LevelUp] = "LevelUp",
            [Id.JobChange] = "JobChanged",
            [Id.ScrollSuccess] = "Enchant/Success",
            [Id.ScrollFailure] = "Enchant/Failure",
            [Id.MonsterCard] = "MonsterBook/cardGet",
        };

        public enum Id
        {
            LevelUp,
            JobChange,
            ScrollSuccess,
            ScrollFailure,
            MonsterCard,
            Length
        }
    }
}