namespace testrobot
{
    public class RaidAttackTarget
    {
        public char Head;
        public char LArm;
        public char RArm;
        public char LHand;
        public char RHand;
        public char Body;
        public char LLeg;
        public char RLeg;
        public string EnemyId;

        public RaidAttackTarget(string enemyId)
        {
            Head='0';
            LArm='0';
            RArm='0';
            LHand='0';
            RHand='0';
            Body='0';
            LLeg='0';
            RLeg='0';
            EnemyId = enemyId;
        }
        public RaidAttackTarget(string enemyId,string s)
        {
            EnemyId = enemyId;
            SetTarget(s);
        }

        public void SetTarget(string s)
        {
            if(s.Length<8)
                return;
            int i = 0;
            Head=s[i++];
            Body=s[i++];
            
            LArm=s[i++];
            RArm=s[i++];
            
            LLeg=s[i++];
            RLeg=s[i++];
            
            LHand=s[i++];
            RHand=s[i++];
        }

        /// <summary>
        /// 设置优先攻击部位
        /// </summary>
        /// <param name="part">部位</param>
        /// <param name="t">
        /// 0-默认
        /// 1-不打
        /// 2-集火
        /// </param>
        public void SetAttack(TitanData.PartName part, int t = 0)
        {
            switch (part)
            {
                case TitanData.PartName.ArmorHead:
                    Head = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorChestUpper:
                    Body = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorArmUpperRight:
                    LArm = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorArmUpperLeft:
                    RArm = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorHandRight:
                    LHand = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorHandLeft:
                    RHand = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorLegUpperRight:
                    LLeg = (char)('0'+t);
                    break;
                case TitanData.PartName.ArmorLegUpperLeft:
                    RLeg = (char)('0'+t);
                    break;
            }
        }

        public bool IsAttack(TitanData.PartName part)
        {
            switch (part)
            {
                case TitanData.PartName.ArmorHead:
                case TitanData.PartName.BodyHead:
                    return Head !='1';
                    break;
                case TitanData.PartName.ArmorChestUpper:
                case TitanData.PartName.BodyChestUpper:
                    return Body !='1';
                    break;
                case TitanData.PartName.ArmorArmUpperRight:
                case TitanData.PartName.BodyArmUpperRight:
                    return LArm !='1';
                    break;
                case TitanData.PartName.ArmorArmUpperLeft:
                case TitanData.PartName.BodyArmUpperLeft:
                    return RArm !='1';
                    break;
                case TitanData.PartName.ArmorHandRight:
                case TitanData.PartName.BodyHandRight:
                    return LHand !='1';
                    break;
                case TitanData.PartName.ArmorHandLeft:
                case TitanData.PartName.BodyHandLeft:
                    return RHand !='1';
                    break;
                case TitanData.PartName.ArmorLegUpperRight:
                case TitanData.PartName.BodyLegUpperRight:
                    return LLeg !='1';
                    break;
                case TitanData.PartName.ArmorLegUpperLeft:
                case TitanData.PartName.BodyLegUpperLeft:
                    return RLeg !='1';
                    break;
            }

            return false;
        }

        public string AttackType(TitanData.PartName part)
        {
            switch (part)
            {
                case TitanData.PartName.ArmorHead:
                case TitanData.PartName.BodyHead:
                    return GetSing(Head);
                    break;
                case TitanData.PartName.ArmorChestUpper:
                case TitanData.PartName.BodyChestUpper:
                    return GetSing(Body);
                    break;
                case TitanData.PartName.ArmorArmUpperRight:
                case TitanData.PartName.BodyArmUpperRight:
                    return GetSing(LArm);
                    break;
                case TitanData.PartName.ArmorArmUpperLeft:
                case TitanData.PartName.BodyArmUpperLeft:
                    return GetSing(RArm);
                    break;
                case TitanData.PartName.ArmorHandRight:
                case TitanData.PartName.BodyHandRight:
                    return GetSing(LHand);
                    break;
                case TitanData.PartName.ArmorHandLeft:
                case TitanData.PartName.BodyHandLeft:
                    return GetSing(RHand);
                    break;
                case TitanData.PartName.ArmorLegUpperRight:
                case TitanData.PartName.BodyLegUpperRight:
                    return GetSing(LLeg);
                    break;
                case TitanData.PartName.ArmorLegUpperLeft:
                case TitanData.PartName.BodyLegUpperLeft:
                    return GetSing(RLeg);
                    break;
            }

            return " ";
        }

        public string GetSing(char mark)
        {
            switch (mark)
            {
                case '1':
                    return "X";
                case '2':
                    return "O";
                default: return " ";
            }
        }

        public string GetTarget()
        {
            return $"{Head}{LArm}{RArm}{LHand}{RHand}{Body}{LLeg}{RLeg}";
        }
    }
}