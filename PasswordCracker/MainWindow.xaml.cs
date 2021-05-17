/*
 * Developer    :   Edgar Elias
 * Version      :   2021.05.27
 * Description  :   The purpose of this program is to test the advantage of using the .NET Task Parallel Library.
 *                  The program will ask the user to enter a password and a time limit to crack the password.
 *                  The program will try to crack the password using a single threaded execution implementation and
 *                  a parallel execution implementation.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordCracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char[] charactersToTest = { };           //create an array with all the valid characters to test
        private bool passwordFound = false;              //will be set to true when the password is found
        private long numOfCombinations = 0;              //to count the number of combinarions
        private string password = string.Empty;          //password entered by the user
        private string crackedPassword = string.Empty;   //to test different combinations
        private DateTime startTime;                      //to set the time when an execution starts
        private double timeLimit = 0;                    //the time limit entered by the user
        private bool exceededTimeSingleExec = false;     //will be set to true if the single execution has exceeded the time limit
        private bool exceededTimeParallelExec = false;   //will be set to true if the parallel execution has exceeded the time limit
        private List<string> pwdTypes = new List<string>(); //store the available password types

        private string testOutput = "";                     //to keep track of the test results
        public MainWindow()
        {
            InitializeComponent();
            
            //Fill Combobox
            BuildListOfPasswordTypes();
            this.passwordTypeCmbBox.ItemsSource = this.pwdTypes;
            this.passwordTypeCmbBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Re-initializes certain class attributes. 
        /// </summary>
        private void ResetClassAttributes()
        {
            this.passwordFound = false;
            this.numOfCombinations = 0;
            this.password = string.Empty;
            this.exceededTimeSingleExec = false;
            this.exceededTimeParallelExec = false;
            this.timeLimit = 0;
            this.resultsLbl.Content = "";
            this.testOutput = string.Empty;

        }

        /// <summary>
        /// Adds to the pwdTypes List the password types that can
        /// be entered to the program. 
        /// </summary>
        private void BuildListOfPasswordTypes()
        {
            this.pwdTypes.Add("Only UPPER-CASE");
            this.pwdTypes.Add("Alphanumeric");
            this.pwdTypes.Add("Numeric");
        }

        /// <summary>
        /// Event handler for when the users clicks the "Crack Password" button. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void crackPwdBtn_Click(object sender, RoutedEventArgs e)
        {

            ResetClassAttributes();

            int selectedPwdType = this.passwordTypeCmbBox.SelectedIndex;
            
            // Validate password
            if(!ValidatePassword(this.passwordTxtBox.Text, selectedPwdType))
            {
                MessageBox.Show($"Invalid Password.\nMake sure the password is {pwdTypes[selectedPwdType]}.", "Invalid Password");
            }

            // Validate time limit
            if(!ValidateTimeLimit(this.timeLimitTxtBox.Text))
            {
                MessageBox.Show($"Invalid Time Limit.\nMake sure the Time Limit is a floating point value.\nExample: 30.0", "Invalid Time Limit");
            }

            //Start Brute Force Attack
            StartTests();
            this.resultsLbl.Content = this.testOutput; //show test output to the user


        }

        /// <summary>
        /// Starts the single threaded and parallel executions.
        /// The tests results are appended to the testOutput
        /// class attribute.
        /// </summary>
        private void StartTests()
        {

            /*** Single Threaded Execution ***/
            this.testOutput += "SINGLE THREAD EXECUTION\n";
            startTime = DateTime.Now;
            BruteForce("", 0);
            if (!exceededTimeSingleExec)
            {
                this.testOutput += $"Cracked Password\t\t: {this.crackedPassword}\n";
                this.testOutput += $"Number of Guesses\t: {numOfCombinations}\n";
                this.testOutput += $"Elapsed\t\t\t: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds\n";
            }
            this.testOutput += "\n\n";

            /*** Parallel Execution ***/
            this.testOutput += "PARALLEL EXECUTION\n";
            startTime = DateTime.Now;
            ParallelBruteForce("", 0);
            if (!exceededTimeParallelExec)
            {
                this.testOutput += $"Cracked Password\t\t: {this.crackedPassword}\n";
                this.testOutput += $"Number of Guesses\t: {numOfCombinations}\n";
                this.testOutput += $"Elapsed\t\t\t: {DateTime.Now.Subtract(startTime).TotalSeconds} seconds\n";
            }

        }

        /// <summary>
        /// Validates through regular expressions if the password that the user typed
        /// matches with the selected password typed in the passwordTypeCmbBox combo box.
        /// </summary>
        /// <param name="password">password that the user entered</param>
        /// <param name="passwordType">Option Index of the password type combo box</param>
        /// <returns>True  if the format is valid</returns>
        private bool ValidatePassword(string password, int passwordType)
        {
            bool valid = false;

            switch (passwordType)
            {
                case 0:
                    var passwordRegex = new Regex(@"\b[A-Z]{3,}\b"); //This password must be at least 3 characters in length, and can be made up of any combination of UPPER-CASE letters
                    if (passwordRegex.IsMatch(password))
                    {
                        this.password = password;
                        this.charactersToTest = CreateUppercaseAlphabetArray();
                        valid = true;
                    }
                    break;
                case 1:
                    //TODO: validate alphanumeric
                    break;

                case 2:
                    //TODO: validate numeric
                    break;

                default:
                    break;
            }

            return valid;
        }


        /// <summary>
        /// Validates if the timeLimitInput is a double.
        /// </summary>
        /// <param name="timeLimitInput">Time limit user input</param>
        /// <returns>True if valid</returns>
        private bool ValidateTimeLimit(string timeLimitInput)
        {
            bool valid = false;

            try
            {
                this.timeLimit = Convert.ToDouble(timeLimitInput);
                valid = true;
            }
            catch (Exception)
            {
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Generates a combination of characters until it matches the given password.
        /// Note that this is a recursive and single threaded method.
        /// </summary>
        /// <param name="tmpCombination">A temporary combination of characters that will be tested on every call</param>
        /// <param name="level">A counter to increment the length of the tmpCombination string</param>
        private void BruteForce(string tmpCombination, int level)
        {
            //Check if the algorithm has exceeded the time limit
            double currExecTime = DateTime.Now.Subtract(startTime).TotalSeconds;
            if (currExecTime > timeLimit)
            {
                this.testOutput +="EXCEEDED TIME LIMIT\n";
                this.testOutput +=$"Current Combination\t: {tmpCombination}\n";
                this.testOutput +=$"Number of Guesses\t: {numOfCombinations}\n";
                this.testOutput +=$"Current Execution Time\t: {currExecTime}\n";
                this.testOutput +=$"Time Limit\t\t: {timeLimit}\n";
                exceededTimeSingleExec = true;
                return;
            }
            else
            {
                level++; //increment the length of the tmpCombination string
                foreach (char c in charactersToTest) //iterate through each character of the charactersToTest
                {
                    string combination = tmpCombination + c; //create new combination and test
                    if ((!passwordFound) && (combination.Length <= password.Length) && (exceededTimeSingleExec == false))
                    {
                        numOfCombinations++;
                        if (combination == password) //combination matches password
                        {
                            this.crackedPassword = combination;
                            passwordFound = true;
                            return;
                        }
                        if (level < charactersToTest.Length)
                        {
                            BruteForce(tmpCombination + c, level); //repeat process
                            if (passwordFound || exceededTimeSingleExec)
                            {
                                //exit method
                                return;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Generates a combination of characters until it matches the given password.
        /// Note that this is a recursive and parallel threaded method.
        /// </summary>
        /// <param name="tmpCombination">A temporary combination of characters that will be tested on every call</param>
        /// <param name="level">A counter to increment the length of the tmpCombination string</param>
        private void ParallelBruteForce(string tmpCombination, int level)
        {
            double currExecTime = DateTime.Now.Subtract(startTime).TotalSeconds;
            if (currExecTime > timeLimit)
            {
                this.testOutput += "EXCEEDED TIME LIMIT\n";
                this.testOutput += $"Current Combination\t: {tmpCombination}\n";
                this.testOutput += $"Number of Guesses\t: {numOfCombinations}\n";
                this.testOutput += $"Current Execution Time\t: {currExecTime}\n";
                this.testOutput += $"Time Limit\t\t: {timeLimit}\n";
                exceededTimeParallelExec = true;
                return;
            }
            else
            {
                level++;
                //TPL Library ForEach
                Parallel.ForEach(charactersToTest, (c) =>
                {

                    string combination = tmpCombination + c;
                    if ((!passwordFound) && (combination.Length <= password.Length) && (exceededTimeParallelExec == false))
                    {
                        numOfCombinations++;
                        if (combination == password)
                        {
                            this.crackedPassword = combination;
                            passwordFound = true;
                            return;
                        }
                        if (level < charactersToTest.Length)
                        {
                            BruteForce(tmpCombination + c, level);
                            if (passwordFound || exceededTimeParallelExec)
                            {
                                return;
                            }
                        }
                    }

                });
            }

        }

        /// <summary>
        /// Creates an array with all the letters of the english alphabet.
        /// </summary>
        /// <returns>Array of type char</returns>
        static char[] CreateUppercaseAlphabetArray()
        {
            char[] alphabet = new char[26];
            int i = 0;
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                alphabet[i] = letter;
                i++;
            }
            return alphabet;
        }

        /// <summary>
        /// Prints a Help/Info message when the user clicks the '?' button. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            string helpMsg = "The purpose of this program is to test the advantage of using the .NET Task Parallel Library.\n\n" +
                             "The program will ask the user to enter a password and a time limit to crack the password.\n\n" +
                             "The program will try to crack the password using a single threaded execution implementation and " +
                             "a parallel execution implementation.";
            MessageBox.Show(helpMsg, "Help");
        }

    } //END OF CLASS
} //END OF NAMESPACE
