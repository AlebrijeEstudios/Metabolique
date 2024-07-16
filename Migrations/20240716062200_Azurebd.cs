using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppVidaSana.Migrations
{
    /// <inheritdoc />
    public partial class Azurebd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.accountID);
                });

            migrationBuilder.CreateTable(
                name: "Breakfasts",
                columns: table => new
                {
                    breakfastID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateBreakfast = table.Column<DateOnly>(type: "date", nullable: false),
                    hourBreakfast = table.Column<TimeOnly>(type: "time", nullable: false),
                    satietyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkedEmotions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breakfasts", x => x.breakfastID);
                    table.ForeignKey(
                        name: "FK_Breakfasts_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dinners",
                columns: table => new
                {
                    dinnerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateDinner = table.Column<DateOnly>(type: "date", nullable: false),
                    hourDinner = table.Column<TimeOnly>(type: "time", nullable: false),
                    satietyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkedEmotions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dinners", x => x.dinnerID);
                    table.ForeignKey(
                        name: "FK_Dinners_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    exerciseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateExercise = table.Column<DateOnly>(type: "date", nullable: false),
                    typeExercise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    intensityExercise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timeSpent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.exerciseID);
                    table.ForeignKey(
                        name: "FK_Exercises_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitsDrink",
                columns: table => new
                {
                    habitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateHabit = table.Column<DateOnly>(type: "date", nullable: false),
                    typeDrink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountConsumed = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitsDrink", x => x.habitID);
                    table.ForeignKey(
                        name: "FK_habitsDrink_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitsDrugs",
                columns: table => new
                {
                    habitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateHabit = table.Column<DateOnly>(type: "date", nullable: false),
                    cigarettesSmoked = table.Column<int>(type: "int", nullable: false),
                    predominantEmotionalState = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitsDrugs", x => x.habitID);
                    table.ForeignKey(
                        name: "FK_habitsDrugs_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitsSleep",
                columns: table => new
                {
                    habitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateHabit = table.Column<DateOnly>(type: "date", nullable: false),
                    sleepHours = table.Column<int>(type: "int", nullable: false),
                    perceptionOfRelaxation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitsSleep", x => x.habitID);
                    table.ForeignKey(
                        name: "FK_habitsSleep_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lunchs",
                columns: table => new
                {
                    lunchID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateLunch = table.Column<DateOnly>(type: "date", nullable: false),
                    hourLunch = table.Column<TimeOnly>(type: "time", nullable: false),
                    satietyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkedEmotions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lunchs", x => x.lunchID);
                    table.ForeignKey(
                        name: "FK_Lunchs_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    mealID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateMeal = table.Column<DateOnly>(type: "date", nullable: false),
                    hourMeal = table.Column<TimeOnly>(type: "time", nullable: false),
                    satietyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkedEmotions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.mealID);
                    table.ForeignKey(
                        name: "FK_Meals_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    medicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateMedication = table.Column<DateOnly>(type: "date", nullable: false),
                    nameMedication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dose = table.Column<int>(type: "int", nullable: false),
                    weeklyFrequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dailyFrequency = table.Column<int>(type: "int", nullable: false),
                    schedule1 = table.Column<TimeOnly>(type: "time", nullable: false),
                    schedule2 = table.Column<TimeOnly>(type: "time", nullable: false),
                    schedule3 = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.medicationID);
                    table.ForeignKey(
                        name: "FK_Medications_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MFUsExcercise",
                columns: table => new
                {
                    monthlyFollowUpID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    question1 = table.Column<int>(type: "int", nullable: false),
                    question2 = table.Column<int>(type: "int", nullable: false),
                    question3 = table.Column<int>(type: "int", nullable: false),
                    question4 = table.Column<int>(type: "int", nullable: false),
                    question5 = table.Column<int>(type: "int", nullable: false),
                    question6 = table.Column<int>(type: "int", nullable: false),
                    question7 = table.Column<int>(type: "int", nullable: false),
                    actWalking = table.Column<float>(type: "real", nullable: false),
                    actModerate = table.Column<float>(type: "real", nullable: false),
                    actVigorous = table.Column<float>(type: "real", nullable: false),
                    totalMET = table.Column<float>(type: "real", nullable: false),
                    sedentaryBehavior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    levelAF = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFUsExcercise", x => x.monthlyFollowUpID);
                    table.ForeignKey(
                        name: "FK_MFUsExcercise_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MFUsHabits",
                columns: table => new
                {
                    monthlyFollowUpID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    answerQuestion1 = table.Column<TimeOnly>(type: "time", nullable: false),
                    answerQuestion2 = table.Column<int>(type: "int", nullable: false),
                    answerQuestion3 = table.Column<TimeOnly>(type: "time", nullable: false),
                    answerQuestion4 = table.Column<int>(type: "int", nullable: false),
                    answerQuestion5a = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5b = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5c = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5d = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5e = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5f = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5g = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5h = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5i = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5j = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion9 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFUsHabits", x => x.monthlyFollowUpID);
                    table.ForeignKey(
                        name: "FK_MFUsHabits_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MFUsMedications",
                columns: table => new
                {
                    monthlyFollowUpID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    answerQuestion1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    treatmentAdherence = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFUsMedications", x => x.monthlyFollowUpID);
                    table.ForeignKey(
                        name: "FK_MFUsMedications_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MFUsNutrition",
                columns: table => new
                {
                    monthlyFollowUpID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    answerQuestion1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    answerQuestion9 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MFUsNutrition", x => x.monthlyFollowUpID);
                    table.ForeignKey(
                        name: "FK_MFUsNutrition_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    birthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    sex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stature = table.Column<int>(type: "int", nullable: false),
                    weigth = table.Column<int>(type: "int", nullable: false),
                    protocolToFollow = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.accountID);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sideEffects",
                columns: table => new
                {
                    sideEffectID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateSideEffect = table.Column<DateOnly>(type: "date", nullable: false),
                    initialSchedule = table.Column<TimeOnly>(type: "time", nullable: false),
                    endSchedule = table.Column<TimeOnly>(type: "time", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sideEffects", x => x.sideEffectID);
                    table.ForeignKey(
                        name: "FK_sideEffects_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Snacks",
                columns: table => new
                {
                    snackID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateSnack = table.Column<DateOnly>(type: "date", nullable: false),
                    hourSnack = table.Column<TimeOnly>(type: "time", nullable: false),
                    satietyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkedEmotions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snacks", x => x.snackID);
                    table.ForeignKey(
                        name: "FK_Snacks_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "foodsBreakfast",
                columns: table => new
                {
                    foodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    breakfastID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nameFood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountConsumed = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foodsBreakfast", x => x.foodID);
                    table.ForeignKey(
                        name: "FK_foodsBreakfast_Breakfasts_breakfastID",
                        column: x => x.breakfastID,
                        principalTable: "Breakfasts",
                        principalColumn: "breakfastID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "foodsDinner",
                columns: table => new
                {
                    foodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dinnerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nameFood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountConsumed = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foodsDinner", x => x.foodID);
                    table.ForeignKey(
                        name: "FK_foodsDinner_Dinners_dinnerID",
                        column: x => x.dinnerID,
                        principalTable: "Dinners",
                        principalColumn: "dinnerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "foodsLunch",
                columns: table => new
                {
                    foodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    lunchID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nameFood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountConsumed = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foodsLunch", x => x.foodID);
                    table.ForeignKey(
                        name: "FK_foodsLunch_Lunchs_lunchID",
                        column: x => x.lunchID,
                        principalTable: "Lunchs",
                        principalColumn: "lunchID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "foodsMeal",
                columns: table => new
                {
                    foodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mealID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nameFood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountConsumed = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foodsMeal", x => x.foodID);
                    table.ForeignKey(
                        name: "FK_foodsMeal_Meals_mealID",
                        column: x => x.mealID,
                        principalTable: "Meals",
                        principalColumn: "mealID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resultsHabits",
                columns: table => new
                {
                    resultsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    monthlyFollowUpID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    resultComponent1 = table.Column<int>(type: "int", nullable: false),
                    resultComponent2 = table.Column<int>(type: "int", nullable: false),
                    resultComponent3 = table.Column<int>(type: "int", nullable: false),
                    resultComponent4 = table.Column<int>(type: "int", nullable: false),
                    resultComponent5 = table.Column<int>(type: "int", nullable: false),
                    resultComponent6 = table.Column<int>(type: "int", nullable: false),
                    resultComponent7 = table.Column<int>(type: "int", nullable: false),
                    globalClassification = table.Column<int>(type: "int", nullable: false),
                    classification = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultsHabits", x => x.resultsID);
                    table.ForeignKey(
                        name: "FK_resultsHabits_MFUsHabits_monthlyFollowUpID",
                        column: x => x.monthlyFollowUpID,
                        principalTable: "MFUsHabits",
                        principalColumn: "monthlyFollowUpID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resultsNutrition",
                columns: table => new
                {
                    resultsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    monthlyFollowUpID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    pointsQuestion1 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion2 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion3 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion4 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion5 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion6 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion7 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion8 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion9 = table.Column<int>(type: "int", nullable: false),
                    pointsQuestion10 = table.Column<int>(type: "int", nullable: false),
                    totalPoints = table.Column<int>(type: "int", nullable: false),
                    classification = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultsNutrition", x => x.resultsID);
                    table.ForeignKey(
                        name: "FK_resultsNutrition_MFUsNutrition_monthlyFollowUpID",
                        column: x => x.monthlyFollowUpID,
                        principalTable: "MFUsNutrition",
                        principalColumn: "monthlyFollowUpID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "foodsSnack",
                columns: table => new
                {
                    foodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    snackID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nameFood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountConsumed = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foodsSnack", x => x.foodID);
                    table.ForeignKey(
                        name: "FK_foodsSnack_Snacks_snackID",
                        column: x => x.snackID,
                        principalTable: "Snacks",
                        principalColumn: "snackID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Breakfasts_accountID",
                table: "Breakfasts",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Dinners_accountID",
                table: "Dinners",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_accountID",
                table: "Exercises",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_foodsBreakfast_breakfastID",
                table: "foodsBreakfast",
                column: "breakfastID");

            migrationBuilder.CreateIndex(
                name: "IX_foodsDinner_dinnerID",
                table: "foodsDinner",
                column: "dinnerID");

            migrationBuilder.CreateIndex(
                name: "IX_foodsLunch_lunchID",
                table: "foodsLunch",
                column: "lunchID");

            migrationBuilder.CreateIndex(
                name: "IX_foodsMeal_mealID",
                table: "foodsMeal",
                column: "mealID");

            migrationBuilder.CreateIndex(
                name: "IX_foodsSnack_snackID",
                table: "foodsSnack",
                column: "snackID");

            migrationBuilder.CreateIndex(
                name: "IX_habitsDrink_accountID",
                table: "habitsDrink",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_habitsDrugs_accountID",
                table: "habitsDrugs",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_habitsSleep_accountID",
                table: "habitsSleep",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Lunchs_accountID",
                table: "Lunchs",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_accountID",
                table: "Meals",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_accountID",
                table: "Medications",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_MFUsExcercise_accountID",
                table: "MFUsExcercise",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_MFUsHabits_accountID",
                table: "MFUsHabits",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_MFUsMedications_accountID",
                table: "MFUsMedications",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_MFUsNutrition_accountID",
                table: "MFUsNutrition",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_resultsHabits_monthlyFollowUpID",
                table: "resultsHabits",
                column: "monthlyFollowUpID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resultsNutrition_monthlyFollowUpID",
                table: "resultsNutrition",
                column: "monthlyFollowUpID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sideEffects_accountID",
                table: "sideEffects",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Snacks_accountID",
                table: "Snacks",
                column: "accountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "foodsBreakfast");

            migrationBuilder.DropTable(
                name: "foodsDinner");

            migrationBuilder.DropTable(
                name: "foodsLunch");

            migrationBuilder.DropTable(
                name: "foodsMeal");

            migrationBuilder.DropTable(
                name: "foodsSnack");

            migrationBuilder.DropTable(
                name: "habitsDrink");

            migrationBuilder.DropTable(
                name: "habitsDrugs");

            migrationBuilder.DropTable(
                name: "habitsSleep");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "MFUsExcercise");

            migrationBuilder.DropTable(
                name: "MFUsMedications");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "resultsHabits");

            migrationBuilder.DropTable(
                name: "resultsNutrition");

            migrationBuilder.DropTable(
                name: "sideEffects");

            migrationBuilder.DropTable(
                name: "Breakfasts");

            migrationBuilder.DropTable(
                name: "Dinners");

            migrationBuilder.DropTable(
                name: "Lunchs");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Snacks");

            migrationBuilder.DropTable(
                name: "MFUsHabits");

            migrationBuilder.DropTable(
                name: "MFUsNutrition");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
