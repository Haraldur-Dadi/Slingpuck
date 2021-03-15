using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour {
    public void ReportGameStarted(bool singlePlayer) {
        if (singlePlayer) {
            Analytics.CustomEvent("1_player_started");
        } else {
            Analytics.CustomEvent("2_player_started");
        }
    }

    public void ReportGameFinishedAnalytics(bool singlePlayer, bool team1Won) {
        if (singlePlayer) {
            Analytics.CustomEvent("1_player_won", new Dictionary<string, object> { 
                {"won", team1Won},
                {"skill rank", Ai.Instance.skillRank}
            });
        } else {
            Analytics.CustomEvent("2_player_finished");
        }
    }
}
