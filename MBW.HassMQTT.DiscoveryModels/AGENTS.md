# Home Assistant MQTT discovery documentation sync

These instructions apply to `MBW.HassMQTT.DiscoveryModels` and its descendants.

## Source of truth

- Model the published `home-assistant/home-assistant.io` MQTT documentation. Do not silently substitute behavior inferred from Home Assistant Core.
- Treat each `{% configuration %}` block as the schema. Review its prose as well as its `type`, `required`, and `default` metadata because allowed values and cross-field constraints are frequently described only in prose.
- Use linked integration documentation for strongly typed values such as device classes.
- Pin one upstream target commit for an entire sync. Do not move the target while entity families are being reviewed.

## Obtain the comparison range

Keep the documentation repository outside this worktree. In PowerShell, create or update a blobless bare clone in the operating-system temporary directory:

```powershell
$haDocsGit = Join-Path ([System.IO.Path]::GetTempPath()) 'mbw-hassmqtt-home-assistant.io.git'

if (Test-Path -LiteralPath $haDocsGit) {
    git --git-dir=$haDocsGit fetch --filter=blob:none origin '+refs/heads/current:refs/heads/current'
} else {
    git clone --bare --filter=blob:none --single-branch --branch current https://github.com/home-assistant/home-assistant.io.git $haDocsGit
}
```

Find the latest completed local synchronization:

```powershell
git log --all --format=full --grep='HA docs sync complete through:' -1
```

Use the SHA following `HA docs sync complete through:` as the baseline. For commits predating that convention, inspect the most recent completed `Update for latest docs` commit and use the right-hand SHA from its Home Assistant documentation compare URL.

Pin and verify the range:

```powershell
$baseline = '<last-completed-docs-sha>'
$target = git --git-dir=$haDocsGit rev-parse refs/heads/current

git --git-dir=$haDocsGit merge-base --is-ancestor $baseline $target
if ($LASTEXITCODE -ne 0) {
    throw "The documentation baseline is not an ancestor of target $target"
}
```

Record `$baseline` and `$target` in the agent's task plan. Do not create a checklist, baseline, report, or snapshot file in this repository.

## Find and inspect relevant changes

List primary documentation changes:

```powershell
git --git-dir=$haDocsGit diff --name-status --find-renames $baseline $target -- `
    'source/_integrations/mqtt.markdown' `
    ':(glob)source/_integrations/*.mqtt.markdown'
```

For each changed document, review both its history and final accumulated diff:

```powershell
$docPath = 'source/_integrations/sensor.mqtt.markdown'

git --git-dir=$haDocsGit log --reverse --format='%H %ad %s' --date=short "$baseline..$target" -- $docPath
git --git-dir=$haDocsGit diff --find-renames $baseline $target -- $docPath
```

Also review linked device-class documentation when its corresponding MQTT model uses a typed enum:

```text
source/_integrations/binary_sensor.markdown
source/_integrations/button.markdown
source/_integrations/event.markdown
source/_integrations/number.markdown
source/_integrations/sensor.markdown
source/_integrations/switch.markdown
source/_integrations/update.markdown
```

Maintain the working checklist in the agent's native task/plan state. Categorize every configuration entry as added, removed, structurally changed, or prose-only changed. A prose-only change still requires review for changed allowed values, nullability, relationships, or validation rules.

## Model design rules

- Put behavior and data shared by every discovery document in `MqttSensorDiscoveryBase` or another appropriate shared base type.
- Represent capabilities shared by some, but not all, entity families through interfaces. Consumers must be able to test for a capability without knowing the concrete entity type.
- Keep canonical property names, types, and XML documentation on capability interfaces or shared value types. Concrete implementations should use `<inheritdoc />` instead of duplicating the documentation.
- Every public discovery configuration property must have XML documentation that accurately reflects the corresponding Home Assistant configuration entry, including documented defaults, allowed values, relationships, and constraints where applicable. This requirement applies to entity-specific properties as well as shared properties.
- During a documentation sync, audit XML-documentation coverage for every discovery model in scope. Do not treat successful compilation or inherited class-level documentation as evidence that its public properties are documented.
- Reuse existing capability interfaces before adding new ones. Extend an interface when the documentation expands the same capability.
- Keep complex repeated structures, such as device and availability data, in shared model types.
- Centralize documented shared validation in the base validator by detecting the corresponding capability interface. Entity validators should contain only entity-specific rules.
- Optional documentation properties should normally remain nullable so omitted JSON lets Home Assistant apply its default. Required properties must be non-nullable and validated.
- Use enums for finite documented wire values. Verify their snake-case or explicit `EnumMember` serialization.
- The `platform` configuration entry belongs to MQTT device-discovery component envelopes. Do not serialize it in standalone component discovery payloads merely because it appears in an entity document.
- When removing or renaming a public property to match the documentation, search all projects and tests for consumers and account for the breaking API change deliberately.

## Verification

- Add serialization tests for exact snake-case keys, enum wire values, null omission, and shared capability properties.
- Add shared validation tests at the capability level and entity-specific tests only for entity-specific constraints.
- Add contract tests where useful to ensure that a model exposing a shared property implements its canonical capability interface with the canonical type.
- Build and test the complete solution after each logical family group and before marking the sync complete.

## Commit history

Keep shared infrastructure and each logical entity family in focused commits. Include the documentation path and pinned range in each commit body, for example:

```text
Update MQTT sensor discovery schema

HA docs path: source/_integrations/sensor.mqtt.markdown
HA docs range: <baseline>..<target>
```

Only after all primary MQTT documents and linked typed-value documents have been reviewed, include this exact marker in the final substantive synchronization commit:

```text
HA docs sync complete through: <target>
```

That marker establishes the baseline for the next full synchronization. A partial entity-family commit does not advance the global baseline.
