using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GreenMemory
{
    class Tests
    {
        /// <summary>
        /// Run all tests.</summary>
        public static void Run()
        {
            if (testMemoryModel())
                System.Diagnostics.Debug.WriteLine("OK: MemoryModel passed all tests.");
            else
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel failed tests.");

            if (testPlayerModel())
                System.Diagnostics.Debug.WriteLine("OK: PlayerModel passed all tests.");
            else
                System.Diagnostics.Debug.WriteLine("ERR: PlayerModel failed tests.");

            // Tests the level setting mechanism only
            if (testAIModel())
                System.Diagnostics.Debug.WriteLine("OK: AIModel passed all tests.");
            else
                System.Diagnostics.Debug.WriteLine("ERR: AIModel failed tests.");

        }

        // Test MemoryModel
        private static bool testMemoryModel() {
            bool testOK = true;

            try
            {
                MemoryModel mmErr = new MemoryModel(7);
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel constructor: No argument thrown.");
            }
            catch (ArgumentException)
            {
                // Should be thrown
            }

            MemoryModel mm = new MemoryModel(6);

            if (mm.NumberOfCards != mm.GetDeck().Length)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: NumberOfCards doesn't match length of GetDeck.");
            }

            int[] deck = mm.GetDeck();
            foreach (int card in deck)
            {
                if (card > 2 || card < 0)
                {
                    testOK = false;
                    System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: Invalid values present in deck.");
                    break;
                }
            }

            /* These tests where valid back when they where relevant :)
            int[] index0 = new int[2];
            int[] index1 = new int[2];
            int[] index2 = new int[2];
            int i0 = 0, i1 = 0, i2 = 0;
            for (int i = 0; i < 6; i++)
            {
                switch (deck[i])
                {
                    case 0:
                        index0[i0++] = i;
                        break;
                    case 1:
                        index1[i1++] = i;
                        break;
                    case 2:
                        index2[i2++] = i;
                        break;
                }
            }

            if (mm.PickTwoCards(index0[0], index1[0]) != null
                || mm.PickTwoCards(index0[1], index1[1]) != null
                || mm.PickTwoCards(index1[0], index2[0]) != null
                || mm.PickTwoCards(index1[1], index2[1]) != null
                || mm.PickTwoCards(index2[0], index0[0]) != null
                || mm.PickTwoCards(index2[1], index0[1]) != null)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: Non-matching cards reported as matching.");
            }

            if (mm.IsGameOver())
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: Game over reported when cards are still on the table.");
            }

            if (mm.PickTwoCards(index0[0], index0[1]) == null
                || mm.PickTwoCards(index1[0], index1[1]) == null
                || mm.PickTwoCards(index2[0], index2[1]) == null)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: Matching cards reported as non-matching.");
            }

            try
            {
                mm.PickTwoCards(0, 1);
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: No exception thrown when removed cards were picked.");
            }
            catch (ArgumentException)
            {
                // Should be thrown
            }

            if (!mm.IsGameOver())
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: MemoryModel: Game over not reported when all cards should be picked.");
            }
            */

            return testOK;
        }

        // Test PlayerModel
        private static bool testPlayerModel()
        {
            bool testOK = true;

            string name = "TestName";
            
            PlayerModel pm = new PlayerModel();
            pm.Name = name;
            
            PlayerModel pm0 = new PlayerModel(name);

            if (pm.Name != pm0.Name || pm.Name != name)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: PlayerModel: Naming doesn't work as expected.");
            }

            pm.AddCollectedPair(1);
            pm.AddCollectedPair(3);

            ReadOnlyCollection<int> pairs = pm.GetCollectedPairs;

            if (pairs.Count != 2 || pairs[0] != 1 || pairs[1] != 3)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: PlayerModel: Error adding and getting collected pairs.");
            }

            if (pm.Score != 2)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: PlayerModel: Wrong score reported.");
            }

            return testOK;
        }

        // Test the AI level setting mechanism
        private static bool testAIModel()
        {
            bool testOK = true;

            AIModel aiModel = new AIModel(null, null, null, null, null, 0, 0);
            PrivateObject accessor = new PrivateObject(aiModel);

            if (aiModel.Level != SettingsModel.AILevel)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Constructor didn't set Level to SettingsModel.AILevel.");
            }

            aiModel.Level = AIModel.Difficulty.Easy;
            if ((double)accessor.GetField("delta") != 0.5)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Setting Level to Easy sets wrong value in delta.");
            }
            if ((AIModel.Difficulty)accessor.GetField("level") != AIModel.Difficulty.Easy)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Private field level is not set to Easy by the Level property.");
            }

            aiModel.Level = AIModel.Difficulty.Hard;
            if ((double)accessor.GetField("delta") != 0.05)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Setting Level to Hard sets wrong value in delta.");
            }
            if ((AIModel.Difficulty)accessor.GetField("level") != AIModel.Difficulty.Hard)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Private field level is not set to Hard by the Level property.");
            }

            aiModel.Level = AIModel.Difficulty.Medium;
            if ((double)accessor.GetField("delta") != 0.1)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Setting Level to Medium sets wrong value in delta.");
            }
            if ((AIModel.Difficulty)accessor.GetField("level") != AIModel.Difficulty.Medium)
            {
                testOK = false;
                System.Diagnostics.Debug.WriteLine("ERR: AIModel: Private field level is not set to Medium by the Level property.");
            }

            return testOK;
        }
    }
}
