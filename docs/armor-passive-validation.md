# Armor passive Windows validation

This note records the CI results for PR [CutFlame/MCDSaveEdit#195](https://github.com/CutFlame/MCDSaveEdit/pull/195) and the outstanding manual checks. The PR remains a draft.

## Verified automated results

On October 19, 2026, [Windows Actions run 39329452612](https://github.com/goodenough7000/MCDSaveEdit/actions/runs/39329452612) passed on commit `63a2d524aa68dd7c7bb36c6c480c1ac985aab6e7`:

- NuGet 6.14.0 restored the solution's packages.config dependencies.
- Visual Studio 2022 MSBuild 17.14.60 rebuilt the complete .NET Framework 4.8 solution in Release / Any CPU: MCDSaveEdit, DungeonTools, PakReader, and MCDSaveEditTests.
- VSTest 17.14.0 (x64), using MSTest 2.2.10, executed **all five ItemExtensionsTests successfully against the built application**.
- `git diff --check` passed.

The five tests cover editing the selected duplicate while preserving order and rarity, an entry removed while the selector is open, an entry belonging to another item, cancelled selection, and missing/empty property arrays.

[Build logs, binary log, and TRX artifact](https://github.com/goodenough7000/MCDSaveEdit/actions/runs/39329452612/artifacts/12223319246) are retained until October 26, 2026. Warnings in unchanged code remain: CS8604 in RelayCommand and AppTests; CA2213 in App. These were not expanded into unrelated fixes.

## CI setup and build fix

The workflow checks out the pinned submodules recursively, restores dependencies from NuGet.org, resolves installed Windows SDK metadata instead of relying on the author's SDK 18362 HintPath, and generates a credential-free CI-only Secrets.cs stub as required by the README.

The Release / Any CPU solution configuration previously skipped the test project. The missing test-project Build.0 entry was added so a full solution build produces the regression-test assembly.

The workflow filters to ItemExtensionsTests and requires TRX counters of five total, executed, and passed tests. It does **not** run the entire test suite; unrelated tests require installed game content or personal save files.

## Remaining in-app verification

**Not performed.** CI uses no game assets or real decryption/analytics credentials. Compilation and model tests do not verify interactive WPF behavior. On Windows with the required game content, use a copy of a save:

- [ ] In the Release app, change the lower of two identical armor passives. Confirm only that row changes, the upper row remains unchanged, and order/rarity are preserved.
- [ ] Cancel selection. Then complete an open selector after removing its selected entry or switching items. Confirm no unintended edit or exception.
- [ ] Save and reopen the copied save; confirm the change, order, and rarities persist.

Keep the PR in draft pending these checks. Issue #187 was not reopened, and nothing was merged.

The GitHub integration could push this branch but returned HTTP 403 for updating the upstream PR description and for adding a PR comment. Browser sign-in was not completed, so this note is the published validation update; the PR description still contains its earlier validation status.
