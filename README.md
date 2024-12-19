# FuseDrill

```yml
name: FuseDrill Fuzzing/Simulation Testing

on:
  push:
    branches:
      - main  # Runs on push to the main branch
  pull_request:
    branches:
      - main  # Runs on pull requests to the main branch

jobs:
  fuzz-test:
    runs-on: ubuntu-latest

    steps:
# ------------ Deploy your app or run inside the GitHub CI/CD 
      - name: Pull FuseDrill test api Docker Image
        run: docker pull ghcr.io/martasp/fusedrill/testapi:latest
        
      - name: Run Test API
        run: |
          docker run -d \
            -e ASPNETCORE_ENVIRONMENT="Development" \
            -p 8080:8080 \
            ghcr.io/martasp/fusedrill/testapi:latest
          
      - name: Wait for Test API to be Ready
        run: |
          # Wait for the API to start and be reachable on port 8080
          until curl -s http://localhost:8080/swagger/v1/swagger.json; do
            echo "Waiting for Test API to start..."
            sleep 5
          done
# ------------------------------------------------------
    - name: Pull FuseDrill Docker Image
      run: docker pull ghcr.io/martasp/fusedrill/fusedrill-cli:latest # Add docker image versioning later

    - name: Run FuseDrill CLI in Docker
      run: |
         docker run --network host --rm \
          -e FUSEDRILL_BASE_ADDRESS="http://localhost:8080/" \
          -e FUSEDRILL_OPENAPI_URL="http://localhost:8080/swagger/v1/swagger.json" \
          -e FUSEDRILL_OAUTH_HEADER="Bearer your-oauth-token"  \
          -e GITHUB_TOKEN="${{ secrets.GITHUB_TOKEN }}" \
          -e SMOKE_FLAG="true" \
          -e GITHUB_REPOSITORY_OWNER="${{ github.repository_owner }}" \
          -e GITHUB_REPOSITORY="${{ github.repository }}" \
          -e GITHUB_HEAD_REF="${{ github.head_ref }}" \
          -e GEMINI_API_KEY="${{ secrets.GEMINI_API_KEY }}" \
          ghcr.io/martasp/fusedrill/fusedrill-cli:latest

      - name: Post-run step
        run: |
          echo "Fuzzing test completed"

```
