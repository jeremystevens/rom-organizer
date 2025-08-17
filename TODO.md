# Known Issues & Fix Plan

The current version of the software has several **known bugs** related to database handling and data consistency.  
Most of these issues stem from how ROM entries are stored, moved, and updated, which sometimes creates duplicate records or inconsistent metadata.

The action items in this document outline the steps required to fix these problems.  
Once completed, they will:
- Eliminate duplicate ROM entries.  
- Ensure ROM moves update existing records rather than creating new ones.  
- Normalize genres into a single, consistent source of truth.  
- Improve data integrity across the database.  
- Provide better history tracking and simplify UI queries.  

By working through this list from easiest to hardest, we can systematically resolve the bugs and stabilize the software.

## TODO List 
---

## ✅ Quick Wins (≤30 min)
- [ ] **Enable FK constraints on every DB open**  
  ETA: 5 min • Ensures no orphan rows.  
  Done when: `PRAGMA foreign_keys=ON` runs on all connections.

- [ ] **Adopt content hash as the canonical ID**  
  ETA: 15–20 min  
  Done when: Scanner/sorter functions always use `content_hash` instead of title/path.

- [ ] **Ban filename-based inserts**  
  ETA: 15–20 min  
  Done when: No `INSERT` statements rely on filename/path; only `content_hash`.

---

## ⚡ Short Tasks (30–90 min)
- [ ] **Create unique constraints & indexes**  
  ETA: 30–45 min  
  Done when:  
  - [ ] `UNIQUE(content_hash)` on `roms`  
  - [ ] `UNIQUE(current_path)` or equivalent in path history  
  - [ ] Supporting indexes added.

- [ ] **Introduce upsert for roms**  
  ETA: 30–45 min  
  Done when: `INSERT ... ON CONFLICT(content_hash) DO UPDATE` implemented.

- [ ] **Add `rom_paths` + trigger to track moves**  
  ETA: 45–60 min  
  Done when: `rom_paths` table exists, trigger records history on path change.

- [ ] **Create `v_roms` view for reads**  
  ETA: 30–45 min  
  Done when: UI pulls from `v_roms` view, not legacy fields.

---

## 🔧 Medium Tasks (2–4 hours)
- [ ] **Implement atomic move flow (transactional)**  
  ETA: 2–3 h  
  Done when: Move = upsert → `File.Move` → `UPDATE path` inside transaction.

- [ ] **Normalize genres (schema + writes)**  
  ETA: 2–3 h  
  Done when: Writes only hit `rom_genres`; legacy genre fields unused.

- [ ] **Backfill `rom_paths` with current entries**  
  ETA: 1–2 h  
  Done when: Every `roms.current_path` has `rom_paths.is_current=1`.

---

## 🛠 Larger Tasks (½–1 day)
- [ ] **Duplicate consolidation by `content_hash`**  
  ETA: 3–6 h  
  Done when: Duplicate roms merged; survivor chosen; paths & FKs preserved.

- [ ] **One-time “Reconcile DB” command**  
  ETA: 3–5 h  
  Done when: Migration handles schema upgrade, path backfill, and duplicate cleanup.

- [ ] **UI swap to `v_roms`**  
  ETA: 3–4 h  
  Done when: All list/detail views read from `v_roms`.

---

## 🚀 Big Refactors (1–2 days)
- [ ] **Genre unification migration**  
  ETA: 6–10 h  
  Done when:  
  - [ ] `roms.genre`, `roms.primary_genre`, and `metadata.genre` collapsed into `genres/rom_genres`  
  - [ ] old columns dropped.

- [ ] **Integrity tests & guardrails**  
  ETA: 6–8 h  
  Done when:  
  - [ ] Tests ensure no duplicate hashes  
  - [ ] unique paths  
  - [ ] valid FKs  
  - [ ] safe moves.

- [ ] **Scanner optimization (rehash policy)**  
  ETA: 6–8 h  
  Done when: Hash only recalculated if size/mtime changed; scan perf validated.

---
