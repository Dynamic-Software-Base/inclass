# Git Workflow Guide

## Part 3: The Full Git Flow (Day-to-Day)

### Normal Feature Flow

#### 1. START — Always branch from develop

```bash
git checkout develop
git pull origin develop
git checkout -b feature/user-authentication
```

#### 2. WORK — Commit often, push daily

```bash
git add .
git commit -m "feat(auth): add JWT token validation"
git push origin feature/user-authentication
```

#### 3. KEEP IN SYNC — Pull develop regularly

```bash
git fetch origin
git rebase origin/develop
# Resolve any conflicts here if needed
git push origin feature/user-authentication --force-with-lease
```

#### 4. OPEN PR → develop

- Fill out the PR template
- CI runs automatically (build + tests)
- Teammate reviews and approves
- Merge using **SQUASH AND MERGE** (keeps history clean)

#### 5. RELEASE TO STAGING

- `develop` → `staging` (PR or automated pipeline trigger)
- Your CD pipeline deploys to staging environment
- QA / smoke tests run

#### 6. RELEASE TO PRODUCTION

- `staging` → `main` (PR with approval)
- Your CD pipeline deploys to production
- Tag the release

---

### Hotfix Flow (Production Bug)

#### 1. Branch from MAIN (not develop)

```bash
git checkout main
git pull origin main
git checkout -b hotfix/fix-payment-null-ref
```

#### 2. Fix it, test it, push it

```bash
git commit -m "fix(payment): handle null reference on checkout"
git push origin hotfix/fix-payment-null-ref
```

#### 3. PR → main (bypass develop for speed)

- Get approval
- CI must pass
- **MERGE** (not squash, so you preserve the fix commit)

#### 4. IMMEDIATELY back-merge to develop

```bash
git checkout develop
git merge main
git push origin develop
# OR open a PR from main → develop
```

---

## Part 4: Commit Message Convention

Use **Conventional Commits** — this is what professional teams use and it enables auto-generated changelogs:

### Format

```
<type>(<scope>): <short description>
```

### Types

| Type | Description |
|------|-------------|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `refactor` | Code change, no feature/fix |
| `test` | Adding/updating tests |
| `chore` | Build process, dependencies |
| `perf` | Performance improvement |
| `ci` | CI/CD pipeline changes |
| `hotfix` | Emergency production fix |

### Examples

```bash
feat(auth): add OAuth2 login with Google
fix(api): return 404 when user not found
chore(deps): update Newtonsoft.Json to 13.0.3
ci(pipeline): add staging deployment stage
hotfix(payment): fix null reference on empty cart
```