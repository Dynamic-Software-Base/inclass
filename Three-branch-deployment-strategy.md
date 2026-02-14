# Three-Branch Deployment Strategy
## Managing `develop` → `staging` → `main`

## Your Branch Structure

```
main (production) ← Protected
  ↑
staging (pre-production) ← Protected
  ↑
develop (integration) ← Default branch, Protected
  ↑
feature/* (temporary branches) ← Auto-deleted after merge
```

---

## Important: Protect Your Long-Lived Branches

**The "auto-delete head branches" setting only deletes the SOURCE branch of a PR, NOT the target branch.**

Example:
- ✅ `feature/login` → `develop` (PR merged) → `feature/login` gets deleted ✅ **Correct!**
- ✅ `develop` → `staging` (PR merged) → `staging` stays, `develop` gets deleted? ❌ **NO!**

**Why?** Because `develop` is the **target** of feature PRs, it won't be deleted. However, to be safe and clear, you should protect all three main branches.

---

## Step 1: Protect Your Main Branches

Go to **GitHub → Settings → Branches → Add branch protection rule**

### Protect `main`
- Branch name pattern: `main`
- ✅ Require a pull request before merging
- ✅ Require approvals (at least 1)
- ✅ Require status checks to pass
- ✅ Do not allow bypassing the above settings
- ✅ Restrict who can push to matching branches (optional: limit to CI/CD or admins)

### Protect `staging`
- Branch name pattern: `staging`
- ✅ Require a pull request before merging
- ✅ Require approvals (at least 1)
- ✅ Require status checks to pass

### Protect `develop` (Default Branch)
- Branch name pattern: `develop`
- ✅ Require a pull request before merging
- ✅ Require approvals (at least 1)
- ✅ Require status checks to pass

**With these protections, your branches CANNOT be deleted accidentally.**

---

## Step 2: Set `develop` as Default Branch

1. Go to **Settings → General → Default branch**
2. Change from `main` to `develop`
3. Click **Update**

Now when developers clone or create PRs, they default to `develop`.

---

## The Deployment Flow

### 🚀 Phase 1: Feature Development (Daily)

**Developer workflow:**

```bash
# Always start from develop
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/user-profile

# Make changes, commit
git add .
git commit -m "feat(profile): add user avatar upload"
git push origin feature/user-profile
```

**On GitHub:**
1. Open PR: `feature/user-profile` → `develop`
2. Get code review approval
3. **Merge using "Squash and merge"**
4. ✅ `feature/user-profile` branch gets auto-deleted (this is expected!)

---

### 🧪 Phase 2: Deploy to Staging (Weekly or When Ready)

When `develop` is stable and ready for QA testing:

**Option A: Using Pull Request (Recommended)**

```bash
# Make sure develop is up to date
git checkout develop
git pull origin develop

# Push develop to remote (if not already pushed)
git push origin develop
```

**On GitHub:**
1. Create PR: `develop` → `staging`
2. **Important:** Use **"Create a merge commit"** (NOT squash!)
    - Why? You want to preserve all the individual feature commits
3. Title: `Release to Staging - [Date] - [Sprint/Version]`
4. Description:
   ```markdown
   ## Features in this release
   - feat(auth): OAuth2 login
   - feat(profile): avatar upload
   - fix(api): handle null user gracefully
   
   ## QA Testing Checklist
   - [ ] Test login flow
   - [ ] Test profile uploads
   - [ ] Smoke tests pass
   ```
5. Merge the PR
6. ✅ **Both `develop` and `staging` branches remain** (protected)
7. Your CI/CD pipeline automatically deploys `staging` branch to staging environment

**Option B: Using Git Command Line (Advanced)**

```bash
# Switch to staging
git checkout staging
git pull origin staging

# Merge develop into staging
git merge develop --no-ff -m "chore(release): deploy to staging [date]"

# Push to trigger deployment
git push origin staging

# Switch back to develop
git checkout develop
```

**After Staging Deployment:**
- QA team tests on staging environment
- Find bugs? Fix them in `develop` and deploy to staging again
- Everything works? Move to production!

---

### 🎯 Phase 3: Deploy to Production (After QA Approval)

When `staging` is tested and approved:

**On GitHub:**
1. Create PR: `staging` → `main`
2. **Use "Create a merge commit"** (NOT squash!)
3. Title: `Production Release v1.2.0 - [Date]`
4. Description:
   ```markdown
   ## Production Release v1.2.0
   
   ### Features
   - OAuth2 login integration
   - User avatar uploads
   
   ### Bug Fixes
   - Fixed null reference in user API
   
   ### QA Verification
   - ✅ All staging tests passed
   - ✅ Load testing completed
   - ✅ Security scan clean
   
   **Approved by:** @qa-lead
   ```
5. Require approval from tech lead or senior developer
6. Merge the PR
7. ✅ **Both `staging` and `main` branches remain** (protected)
8. Your CI/CD pipeline automatically deploys `main` branch to production

**After Production Deployment:**

```bash
# Tag the release
git checkout main
git pull origin main
git tag -a v1.2.0 -m "Production release v1.2.0 - OAuth2 and avatars"
git push origin v1.2.0

# Sync main back to develop (to ensure they stay aligned)
git checkout develop
git merge main --no-ff -m "chore: sync main back to develop after release"
git push origin develop
```

---

## 🚨 Hotfix Flow (Production Emergency)

When production has a critical bug:

```bash
# Branch from main (production)
git checkout main
git pull origin main
git checkout -b hotfix/critical-payment-bug

# Fix the bug
git add .
git commit -m "hotfix(payment): fix null reference on checkout"
git push origin hotfix/critical-payment-bug
```

**On GitHub:**
1. Create PR: `hotfix/critical-payment-bug` → `main`
2. **Use "Create a merge commit"** (preserve the hotfix)
3. Get emergency approval
4. Merge to `main` (deploys to production immediately)
5. ✅ `hotfix/critical-payment-bug` gets auto-deleted (expected)

**CRITICAL: Back-merge to staging and develop**

```bash
# Merge to staging
git checkout staging
git pull origin staging
git merge main --no-ff -m "chore: merge hotfix from main"
git push origin staging

# Merge to develop
git checkout develop
git pull origin develop
git merge main --no-ff -m "chore: merge hotfix from main"
git push origin develop
```

**Or via PRs:**
1. Create PR: `main` → `staging` (merge commit)
2. Create PR: `main` → `develop` (merge commit)

This ensures the hotfix is in all branches!

---

## Summary: When Branches Get Deleted

| Branch Type | Gets Deleted? | Why? |
|------------|---------------|------|
| `feature/*` | ✅ Yes | Temporary, auto-deleted after merge to `develop` |
| `bugfix/*` | ✅ Yes | Temporary, auto-deleted after merge to `develop` |
| `hotfix/*` | ✅ Yes | Temporary, auto-deleted after merge to `main` |
| `develop` | ❌ Never | Protected long-lived branch |
| `staging` | ❌ Never | Protected long-lived branch |
| `main` | ❌ Never | Protected long-lived branch |

**The auto-delete setting is PERFECT for your workflow** because:
- ✅ Cleans up temporary feature/bugfix/hotfix branches automatically
- ✅ Never touches `develop`, `staging`, or `main` (they're protected)
- ✅ Keeps your branch list clean

---

## Quick Reference Commands

### Deploy develop to staging
```bash
# Via PR (recommended)
# Just create PR on GitHub: develop → staging

# Via command line
git checkout staging
git pull origin staging
git merge develop --no-ff -m "chore(release): deploy to staging $(date +%Y-%m-%d)"
git push origin staging
```

### Deploy staging to production
```bash
# Via PR (recommended)
# Create PR on GitHub: staging → main

# Via command line
git checkout main
git pull origin main
git merge staging --no-ff -m "chore(release): deploy to production v1.2.0"
git push origin main
git tag -a v1.2.0 -m "Release v1.2.0"
git push origin v1.2.0

# Sync back to develop
git checkout develop
git merge main --no-ff
git push origin develop
```

### Check current branch status
```bash
git fetch --all
git branch -vv
```

---

## GitHub Actions Example (Optional CI/CD)

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy

on:
  push:
    branches:
      - staging
      - main

jobs:
  deploy-staging:
    if: github.ref == 'refs/heads/staging'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy to Staging
        run: |
          echo "Deploying to staging environment..."
          # Add your deployment commands here

  deploy-production:
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy to Production
        run: |
          echo "Deploying to production environment..."
          # Add your deployment commands here
```

This automatically deploys when you merge to `staging` or `main`!

---

## Best Practices

1. **Always use PRs** for `develop` → `staging` → `main` (never force push)
2. **Use "Create a merge commit"** for release PRs (preserves history)
3. **Use "Squash and merge"** for feature PRs (clean commit history)
4. **Tag production releases** with semantic versioning (v1.2.0)
5. **Document what's in each release** in the PR description
6. **Test on staging** before production (always!)
7. **Sync hotfixes back** to all branches immediately

Your branches will never be accidentally deleted with this setup! 🎉