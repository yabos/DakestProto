﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionCntWin : HeroBattleAction
{
    public override void Initialize(Hero owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
        ReadCommend(Hero.EActionCommend.COMMEND_CNT_WIN);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        UtilFunc.ChangeLayersRecursively(m_Owner.transform, Define.BATTLE_ACTION_LAYER);

        m_Owner.StartCoroutine(ActionProc());
    }

    public override void DoEnd(EHeroBattleAction eNextAction)
    {
        base.DoEnd(eNextAction);

        m_Owner.StopCoroutine(ActionProc());
    }
}