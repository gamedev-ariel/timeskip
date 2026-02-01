Data processing — What we collect and how it is sent to Supabase and the model API

Overview
- The game logs structured events while you play. At session end, a minimal summary JSON is sent to Supabase, and a numeric feature vector is sent to a prediction model API (Hugging Face/Router or another HTTP endpoint).
- No raw screenshots or personal data are uploaded by default. Only the session summary line and the scenes‑keyed event JSON structure are included in the Supabase row. The model API receives only numeric features.

When data is exported
- Export occurs when a session concludes and persistence is triggered by the logger’s save/export routine.

Supabase upload
- Endpoint: `${supabaseUrl}/rest/v1/${supabaseTable}`
- Method: POST
- Headers:
  - `Content-Type: application/json`
  - `apikey: <supabaseAnonKey>`
  - `Authorization: Bearer <supabaseAnonKey>`
- Row body (fields):
  - `player_name`: value from logger configuration.
  - `session_id`: per‑session GUID.
  - `session_json`: a compact export that begins with a human‑readable one‑line summary, followed by scenes‑keyed arrays of event objects and minigame sections. By request, only the minimal summary line is included in the row body for Supabase in this build.
- Summary line content (same order as features below):
  - `screws_spawned`, `screws_collected`
  - `forest_fish_collisions`, `forest_out_of_bounds`, `forest_5_screws_collected`, `forest_approaches`
  - `target_rt_median`, `target_rt_cv`
  - `forest_keys`, `forest_key_gap_mean`, `forest_key_gap_cv`
  - `river_fish_collisions`, `river_out_of_bounds`, `river_win`, `river_approaches`
  - `river_keys`, `river_key_gap_mean`, `river_key_gap_cv`
  - `dwarf_enters`, `dwarf_wm_rt_median`, `dwarf_wm_rt_cv`, `dwarf_correct`, `dwarf_incorrect`

Model API upload (Hugging Face/Router or custom)
- If `modelApiUrl` is configured, the logger also sends a numeric feature vector for prediction.
- Request body format:
```
{
  "data": [[f0, f1, ..., f22]],
  // Optionally when using HF Router URLs: "fn_index": 0
}
```
- Auth header: if `modelApiBearerToken` is non‑empty, `Authorization: Bearer <token>` is added.
- Timeout: 20 seconds.
- Response handling:
  - The logger prints the first 500 chars of the response for diagnostics.
  - It attempts to parse a compact schema `{"prediction":[0|1], "probabilities":[[p0, p1]]}` and, if recognized, overlays a two‑line message on screen: either "ADHD detected." or "No ADHD detected." with the corresponding probability.

23‑feature vector (order and meaning)
The feature vector is built in the exact order below, matching the summary line and contributing to the model request:
0. screws_spawned
1. screws_collected
2. forest_fish_collisions
3. forest_out_of_bounds
4. forest_5_screws_collected
5. forest_approaches
6. target_rt_median
7. target_rt_cv
8. forest_keys
9. forest_key_gap_mean
10. forest_key_gap_cv
11. river_fish_collisions
12. river_out_of_bounds
13. river_win
14. river_approaches
15. river_keys
16. river_key_gap_mean
17. river_key_gap_cv
18. dwarf_enters
19. dwarf_wm_rt_median
20. dwarf_wm_rt_cv
21. dwarf_correct
22. dwarf_incorrect

What we log during play (by area)
- Forest
  - Item lifecycle: `item_spawn` (screw/blueberry), `item_resolve` (collected/expired/etc.).
  - Outcomes: fish/water collisions, out‑of‑bounds losses; win on collecting all screws.
  - Approaches: proximity‑based approach events toward blueberry/fish.
  - Input metrics: number of keypresses and inter‑key gap mean/CV.
  - Target reaction time metrics (median and CV) from target interactions.
- River
  - Jumps: `jump_start` (from rock id with generated jump id), `jump_land` (to rock id with same jump id).
  - Hazards/outcomes: fish and water collisions, out‑of‑bounds; `river_finish` with result win/lose.
  - Approaches: toward fish.
  - Input metrics: number of keypresses and inter‑key gap mean/CV.
- Dwarf’s House (Memory game)
  - Minigame lifecycle: `dwarf_minigame_start`, `dwarf_minigame_end` (with reason).
  - Per level: `memory_level_start`, `dwarf_trial_start` (shown options + correct), `dwarf_trial_response` (selected item, correctness, RT ms), `memory_level_result`.

Configuration (where to set URLs and keys)
- Supabase
  - `supabaseUrl` (e.g., https://<your-project-ref>.supabase.co)
  - `supabaseAnonKey` (anon key with insert permissions on your table)
  - `supabaseTable` (target PostgREST table name)
- Model API
  - `modelApiUrl` (HF Inference Endpoint, HF Router, or custom service URL)
  - `modelApiBearerToken` (optional; required for private endpoints)

On‑screen prediction overlay
- After a successful model response matching the compact schema, a two‑line overlay is displayed: the label and probability. This does not affect gameplay or data export.

Related scripts (for reference)
- Assets/Scripts/ExperimentLogger/ExperimentLogger.Persistence.cs (Supabase upload, feature vector, model API request/response)
- Assets/Scripts/ExperimentLogger/ExperimentDataModels.cs (event data structures)
- Assets/Scripts/ExperimentLogger/ExperimentLogger.cs and ExperimentLogger.*.cs (event emission by area)
