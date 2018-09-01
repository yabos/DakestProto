﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBattleUI
{
    Win,
    Lose,
    Max
}

public class UIBattle : UIBase
{
    BattleScene Owner;

    Transform BattleLoading = null;
    Transform HeroHp = null;
    Transform DamageRoot = null;

    BattleProfile[] Profiles = new BattleProfile[2];
    GameObject GoTurnTimer;
    TurnTimer TurnTime;

    Dictionary<eBattleUI, UIBase> DicBattleUI = new Dictionary<eBattleUI, UIBase>();

    #region IBhvUpdatable

    public override void BhvOnEnter()
    {
        Owner = OwnerScene as BattleScene;

        HeroHp = transform.Find("Anchor/HeroHP");
        BattleLoading = transform.Find("Anchor/Loading");
        DamageRoot = transform.Find("Anchor/DamageRoot");

        var tranProfile = transform.Find("Anchor_BL/Profile");
        if (tranProfile != null)
        {
            Profiles[0] = tranProfile.GetComponent<BattleProfile>();
        }

        tranProfile = transform.Find("Anchor_BR/Profile");
        if (tranProfile != null)
        {
            Profiles[1] = tranProfile.GetComponent<BattleProfile>();
        }

        GoTurnTimer = transform.Find("Anchor/Timer").gameObject;
        if (GoTurnTimer != null)
        {
            TurnTime = GoTurnTimer.GetComponent<TurnTimer>();
        }

        SetBattleUI();
    }

    public override void BhvOnLeave() { }

    public override void BhvFixedUpdate(float dt)
    {
    }

    public override void BhvLateFixedUpdate(float dt)
    {
    }

    public override void BhvUpdate(float dt)
    {
    }

    public override void BhvLateUpdate(float dt)
    {
    }

    #endregion // "IBhvUpdatable"

    protected override void ShowWidget(params object[] data) { }
    protected override void HideWidget() { }

    public override void OnNotify(INotify message)
    {

    }    

    public void OnNextLevel()
    {
        Global.SceneMgr.Transition<LobbyScene>("LobbyScene", 0.5f, 0.3f, (code) =>
        {
            Global.SceneMgr.LogWarning(StringUtil.Format("Scene Transition -> {0}", "LobbyScene"));
        });
    }


    void SetBattleUI()
    {
        AddBattleUI(eBattleUI.Win, ResourcePath.BattleUIWinPath);
        AddBattleUI(eBattleUI.Lose, ResourcePath.BattleUILosePath);
    }

    void AddBattleUI(eBattleUI type, string Path)
    {
        var goUI = Global.ResourceMgr.CreateUIResource(Path, true);
        if (goUI != null)
        {
            GameObject uiRoot = Instantiate(goUI.ResourceData) as GameObject;
            if (uiRoot != null)
            {
                uiRoot.transform.name = uiRoot.name;
                uiRoot.transform.parent = GameObject.FindGameObjectWithTag("UICamera").transform;

                uiRoot.transform.localPosition = Vector3.zero;
                uiRoot.transform.localRotation = Quaternion.identity;
                uiRoot.transform.localScale = Vector3.one;

                var baseUI = uiRoot.GetComponent<UIBase>();
                DicBattleUI.Add(type, baseUI);

                uiRoot.SetActive(false);
            }
        }
    }

    public void ActiveUI(eBattleUI type, bool active)
    {
        if (DicBattleUI.ContainsKey(type))
        {
            if (DicBattleUI[type].gameObject.activeSelf != active)
            {
                DicBattleUI[type].gameObject.SetActive(active);
            }
        }
        else
        {
            Debug.LogError("Load Failed UI : " + type.ToString());
        }
    }

    //public override void SendEvent(EBattleEvent uIEvent)
    //{
    //    if (uIEvent == EBattleEvent.UIEVENT_SELECT_TARGET)
    //    {
    //        SetBattleSelActionType();
    //    }
    //    else if (uIEvent == EBattleEvent.UIEVENT_ACTION_ATK)
    //    {
    //        SetHeroActionType(Hero.EAtionType.ACTION_ATK);
    //    }
    //    else if (uIEvent == EBattleEvent.UIEVENT_ACTION_COUNT)
    //    {
    //        SetHeroActionType(Hero.EAtionType.ACTION_COUNT);
    //    }
    //    else if (uIEvent == EBattleEvent.UIEVENT_ACTION_FAKE)
    //    {
    //        SetHeroActionType(Hero.EAtionType.ACTION_FAKE);
    //    }
    //}

    public void SetBattleSelActionType()
    {
        
        var profile = GetProfile(Owner.ActiveTargetHeroNo);
        if (profile != null)
        {
            profile.TweenPosSpriteProfile(true);
            ActiveSelActionType(true, true);
            SetTurnTimer(Define.SELECT_ACTIONTYPE_LIMITTIME, ETurnTimeType.TURNTIME_SEL_ACTIONTYPE);

            Owner.OnlyActionInput = true;
        }
    }

    public void SetHeroActionType(Hero.EAtionType eAtionType)
    {
        int heroNo = Owner.ActiveTurnHeroNo;
        if (Owner.GetActiveHeroTeam() == false)
        {
            heroNo = Owner.ActiveTargetHeroNo;
        }

        var heroCont = BattleHeroManager.Instance.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            heroCont.ActionType = eAtionType;
        }

        // 원래는 상대방의 입력 정보를 알아와야되는데
        // 지금은 AI로 대체 . 랜덤으로 타입을 정해준다.
        heroNo = Owner.ActiveTargetHeroNo;
        if (Owner.GetActiveHeroTeam() == false)
        {
            heroNo = Owner.ActiveTurnHeroNo;
        }
        Owner.BattleAIManager.SetRandomActionType(heroNo);

        Owner.BattleStateManager.ChangeState(EBattleState.BattleState_Action);
        Owner.OnlyActionInput = false;
    }

    public void ActiveLoadingIMG(bool bActive)
    {
        BattleLoading.gameObject.SetActive(bActive);
    }

    public void CreateHeroHp(System.Guid uid, bool bMyTeam)
    {
        var goHPRes = Global.ResourceMgr.CreateUIResource("UI/Common/Prefabs/HPGauge", false);
        if (goHPRes == null) return;

        GameObject goHP = Instantiate(goHPRes.ResourceData) as GameObject;
        if (goHP != null)
        {
            goHP.transform.parent = HeroHp.transform;
            goHP.transform.name = uid.ToString();

            goHP.transform.position = Vector3.zero;
            goHP.transform.rotation = Quaternion.identity;
            goHP.transform.localScale = new Vector3(0.8f, 0.8f, 1);

            goHP.SetActive(true);
        }
    }

    public void UpdateHPGauge(System.Guid uid, float fFillAmountHp)
    {
        if (HeroHp == null) return;

        for (int i = 0; i < HeroHp.childCount; ++i)
        {
            Transform tChild = HeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                Transform tSlider = tChild.Find("SpriteSlider");
                if (tSlider == null) continue;
                UISprite sprite = tSlider.GetComponent<UISprite>();
                if (sprite == null) continue;
                sprite.fillAmount = fFillAmountHp;
            }
        }
    }

    public void UpdatePosHPGauge(System.Guid uid, Transform tEf_HP)
    {
        if (HeroHp == null) return;

        for (int i = 0; i < HeroHp.childCount; ++i)
        {
            Transform tChild = HeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                Vector3 vScreenPos = Camera.main.WorldToScreenPoint(tEf_HP.position);
                tChild.position = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(vScreenPos.x, vScreenPos.y, 0));
            }
        }
    }

    public void DestroyHPGauge(System.Guid uid)
    {
        if (HeroHp == null) return;

        for (int i = 0; i < HeroHp.childCount; ++i)
        {
            Transform tChild = HeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                NGUITools.Destroy(tChild.gameObject);
            }
        }
    }

    BattleProfile GetProfile(int heroNo)
    {
        var heroCont = BattleHeroManager.Instance.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            BattleProfile bp = null;
            if (heroCont.IsMyTeam)
            {
                bp = Profiles[0];

            }
            else
            {
                bp = Profiles[1];
            }

            return bp;
        }

        return null;
    }

    // 실제 공격 타입을 선택하는 UI 
    public void ActiveSelActionType(bool active, bool myTurn = false)
    {
        Profiles[0].ActiveSelActionType(active, myTurn);
    }

    public void SetTurnTimer(float fTime, ETurnTimeType type)
    {
        TurnTime.SetTimer(this, fTime, type);
        ActiveTurnTimer(true);
    }

    public void ActiveHUDUI(bool active)
    {
        Owner.TurnUI.ActiveTurnUI(active);
        ActiveHPUI(active);
    }

    public void SetProfileUI(int heroNo, bool isActiveHero)
    {
        var bp = GetProfile(heroNo);
        if (bp != null)
        {
            var heroCont = BattleHeroManager.Instance.GetHeroControl(heroNo);
            if (heroCont != null)
            {
                bp.SetProfile(heroCont, isActiveHero);
            }
        }
    }

    public void SetReadyStateProfileUI(Hero heroCont)
    {
        var bp = GetProfile(heroCont.HeroNo);
        if (bp != null)
        {
            bp.BattleStateReadyOnlyProfile(heroCont);
        }
    }

    void ActiveHPUI(bool active)
    {
        HeroHp.gameObject.SetActive(active);
    }

    public void ActiveTurnTimer(bool active)
    {
        GoTurnTimer.SetActive(active);
    }

    public void ActiveAllBattleProfile(bool active)
    {
        Profiles[0].ActiveProfile(active);
        Profiles[1].ActiveProfile(active);
    }

    public void ActiveBattleProfile(bool active, bool myTeam)
    {
        if (myTeam)
        {
            Profiles[0].gameObject.SetActive(active);
        }
        else
        {
            Profiles[1].gameObject.SetActive(active);
        }
    }

    public void CreateDamage(int iDamage, Vector3 vPos, bool bMyTeam)
    {
        var goDamageRes = Global.ResourceMgr.CreateUIResource("UI/Common/Prefabs/HeroDamage", false);
        if (goDamageRes != null)
        {
            GameObject goDamage = Instantiate(goDamageRes.ResourceData) as GameObject;
            if (goDamage != null)
            {
                goDamage.transform.parent = DamageRoot;

                Vector2 vScreenPos = Camera.main.WorldToScreenPoint(new Vector2(vPos.x, vPos.y));
                goDamage.transform.position = UICamera.currentCamera.ScreenToWorldPoint(new Vector2(vScreenPos.x, vScreenPos.y));
                goDamage.transform.localPosition = new Vector3(goDamage.transform.localPosition.x, goDamage.transform.localPosition.y, 0);
                goDamage.transform.localRotation = Quaternion.identity;
                goDamage.transform.localScale = Vector3.one;
                Damage hd = goDamage.GetComponent<Damage>();
                if (hd != null)
                {
                    hd.m_LabelDamage.text = iDamage.ToString();
                }
            }
        }
    }
}