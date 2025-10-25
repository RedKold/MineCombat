using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MineCombat;
using System;
using static MineCombat.EventManager;
using UnityEngine.Assertions;

public class ModSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Mod System Initialized.");
        ModLoder.LoadMod();
        EventManager.BuildConst();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public static class ModLoder
{
    public static void LoadMod()
    {
        Events["TestEvent1"].Bind(() => { Debug.Log("TestEvent1调用了优先级为-5的方法"); }, -5);
        Events["TestEvent1"].Bind(() => { Debug.Log("TestEvent1调用了优先级为3的方法"); }, 3);
        Events["TestEvent1"].Bind(() => { Debug.Log("TestEvent1调用了优先级为-25的方法"); }, -25);
        Events["TestEvent1"].Bind(() => { Debug.Log("TestEvent1调用了优先级为0的方法"); });

        Events["TestEvent2"].Bind(((string, string) item) => { (string str1, string str2) = item; Debug.Log(str1 + "和" + str2 + "的第1次亮相，但优先级是6"); }, 6);

        Events["TestEvent3"].Bind((int i1, int i2) => { i1 = 26; i2 = 48; Debug.Log($"TryChangeNumber, and i1 = {i1}, i2 = {i2}"); });

        SlicedEvents["TestSlicedEvent1"].CreateBranch("branch1");
        SlicedEvents["TestSlicedEvent1"].CreateBranch("branch2");
        SlicedEvents["TestSlicedEvent1"].Bind("branch1", () => { Debug.Log("branch1调用了方法1"); });
        SlicedEvents["TestSlicedEvent1"].Bind("branch1", () => { Debug.Log("branch1调用了方法2"); });
        SlicedEvents["TestSlicedEvent1"].Bind("branch2", () => { Debug.Log("branch2调用了方法1"); }, 9);
        SlicedEvents["TestSlicedEvent1"].Bind("branch2", () => { Debug.Log("branch2调用了方法2"); });

        RandomEvents["TestRandomEvent1"].CreateItem("storm", 1, () => { Debug.Log("随机事件：暴风骤雨"); });
        RandomEvents["TestRandomEvent1"].CreateItem("hurricane", 2, () => { Debug.Log("随机事件：台风来袭"); });
        RandomEvents["TestRandomEvent1"].CreateItem("tsunami", 2, () => { Debug.Log("随机事件：海啸肆虐"); });
        RandomEvents["TestRandomEvent1"].CreateItem("volcanic-eruption", 5, () => { Debug.Log("随机事件：火山爆发"); });

        // 注册事件
        Events["CombatantDied"].Bind(new Action<Player>(c =>
        {
            var view = CombatantView.AllViews.Find(v => v.player == c);
            Debug.Log("Finding view for " + c.Name);
            if (view != null)
            {
                view.SetSelected(false);
                view.ShowWrapper(false);
                Debug.Log($"{c.Name} has died.");
            }
            Assert.IsNotNull(view, "CombatantView should not be null on CombatantDied event");
        }));

        Events["HealthChanged"].Bind(new Action<Player>(c =>
        {
            var view = CombatantView.AllViews.Find(v => v.player == c);
            if (view != null)
            {
                view.UpdateHealthDisplay();
            }
            Assert.IsNotNull(view, "CombatantView should not be null on CombatantDied event");
        })); 

    }
}
