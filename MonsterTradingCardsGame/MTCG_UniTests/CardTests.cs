using MonsterTradingCardsGame.Model;
using MonsterTradingCardsGame.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_UnitTests
{
    [TestClass]
    public class CardTests
    {
        private User user;

        private Spell normalSpell;
        private Spell fireSpell;

        private Monster normalMonster;
        private Monster windMonster;
        private Monster lightningMonster;
        private Monster earthMonster;


        [TestInitialize]
        public void Init()
        {
            user = new User(1, "Max", "Muster", "MM");

            normalSpell = new Spell(10, EElementType.NORMAL, 1);
            fireSpell = new Spell(10, EElementType.FIRE, 1);

            normalMonster = new Monster(10, EElementType.NORMAL, 100);
            windMonster = new Monster(10, EElementType.WIND, 100);
            lightningMonster = new Monster(10, EElementType.LIGHTNING, 100);
            earthMonster = new Monster(10, EElementType.EARTH, 100);
        }

        [TestMethod]
        public void SpellAttacksUser()
        {
            //User allways takes normal dmg
            normalSpell.Attack(user);
            Assert.AreEqual(user.MaxHP - normalSpell.DMG, user.HP);
            Assert.IsTrue(normalSpell.Uses < normalSpell.MaxUses);

            user.ResetHP();

            fireSpell.Attack(user);
            Assert.AreEqual(user.MaxHP - fireSpell.DMG, user.HP);
            Assert.IsTrue(fireSpell.Uses < fireSpell.MaxUses);
        }

        [TestMethod]
        public void MonsterAttacksUser()
        {
            //User allways takes normal dmg
            normalMonster.Attack(user);
            Assert.AreEqual(user.MaxHP - normalMonster.DMG, user.HP);

            user.ResetHP();

            windMonster.Attack(user);
            Assert.AreEqual(user.MaxHP - windMonster.DMG, user.HP);
        }

        [TestMethod]
        public void SpellAttacksMonster()
        {
            //Normal Spell always deals normal dmg
            normalSpell.Attack(normalMonster);
            Assert.AreEqual(normalMonster.MaxHP - normalSpell.DMG, normalMonster.HP);
            Assert.IsTrue(normalSpell.Uses < normalSpell.MaxUses);
            normalSpell.ResetStats();
            normalMonster.ResetStats();

            normalSpell.Attack(windMonster);
            Assert.AreEqual(windMonster.MaxHP - normalSpell.DMG, windMonster.HP);
            Assert.IsTrue(normalSpell.Uses < normalSpell.MaxUses);
            normalSpell.ResetStats();
            windMonster.ResetStats();

            //Normal Monster always get normal dmg
            fireSpell.Attack(normalMonster);
            Assert.AreEqual(normalMonster.MaxHP - fireSpell.DMG, normalMonster.HP);
            Assert.IsTrue(fireSpell.Uses < fireSpell.MaxUses);
            fireSpell.ResetStats();
            normalMonster.ResetStats();

            //Wind Monster gets more dmg from Fire Spell
            fireSpell.Attack(windMonster);
            Assert.IsTrue(windMonster.MaxHP - fireSpell.DMG > windMonster.HP);
            Assert.IsTrue(fireSpell.Uses < fireSpell.MaxUses);
            fireSpell.ResetStats();
            windMonster.ResetStats();

            //Earth Monster gets less dmg from Fire Spell
            fireSpell.Attack(earthMonster);
            Assert.IsTrue(earthMonster.MaxHP - fireSpell.DMG < earthMonster.HP);
            Assert.IsTrue(fireSpell.Uses < fireSpell.MaxUses);
            fireSpell.ResetStats();
            earthMonster.ResetStats();

            //Lightning Monster gets normal dmg from Fire Spell
            fireSpell.Attack(lightningMonster);
            Assert.AreEqual(lightningMonster.MaxHP - fireSpell.DMG, lightningMonster.HP);
            Assert.IsTrue(fireSpell.Uses < fireSpell.MaxUses);
            lightningMonster.ResetStats();

        }

        [TestMethod]
        public void MonsterAttacksMonster()
        {
            //Normal Monster always deals normal dmg
            normalMonster.Attack(windMonster);
            Assert.AreEqual(windMonster.MaxHP - normalMonster.DMG, windMonster.HP);
            windMonster.ResetStats();

            normalMonster.Attack(lightningMonster);
            Assert.AreEqual(lightningMonster.MaxHP - normalMonster.DMG, lightningMonster.HP);
            lightningMonster.ResetStats();

            //Normal Monster always get normal dmg
            windMonster.Attack(normalMonster);
            Assert.AreEqual(normalMonster.MaxHP - windMonster.DMG, normalMonster.HP);
            normalMonster.ResetStats();

            //Lightning Monster deals less dmg to Wind Monster
            lightningMonster.Attack(windMonster);
            Assert.IsTrue(windMonster.MaxHP - lightningMonster.DMG < windMonster.HP);
            windMonster.ResetStats();

            //Wind Monster deals more dmg to Lightning Monster
            windMonster.Attack(lightningMonster);
            Assert.IsTrue(lightningMonster.MaxHP - windMonster.DMG > lightningMonster.HP);
            lightningMonster.ResetStats();

            //Earth and Lightning Monster deal normal dmg to each other
            lightningMonster.Attack(earthMonster);
            Assert.AreEqual(earthMonster.MaxHP - lightningMonster.DMG, earthMonster.HP);
            earthMonster.ResetStats();


            earthMonster.Attack(lightningMonster);
            Assert.AreEqual(lightningMonster.MaxHP - earthMonster.DMG, lightningMonster.HP);
            lightningMonster.ResetStats();
        }

        [TestMethod]
        public void SpellCanNotAttackMoreThanUses()
        {
            normalSpell.Attack(user);
            Assert.AreEqual(user.MaxHP - normalSpell.DMG, user.HP);
            Assert.AreEqual(0, normalSpell.Uses);

            user.ResetHP();

            normalSpell.Attack(user);
            Assert.AreEqual(user.MaxHP, user.HP);
            Assert.AreEqual(0, normalSpell.Uses);
        }
    }
}
