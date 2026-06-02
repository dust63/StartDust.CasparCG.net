DOTNET ?= dotnet
CONFIGURATION ?= Release
SOLUTION ?= src/StarDust.CasparCG.net.sln
HOSTING_PROJECT ?= src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj
TESTING_PROJECT ?= test/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj
UNIT_TESTS ?= test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj
INTEGRATION_TESTS ?= test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj
RUNTIME_PROJECT ?= src/StarDust.CasparCG/StarDust.CasparCG.csproj
PACKAGE_OUTPUT ?= artifacts/packages

.PHONY: test lint clean build publish

test:
	$(DOTNET) test $(UNIT_TESTS) -c $(CONFIGURATION) -m:1 -p:BuildInParallel=false -nr:false
	$(DOTNET) test $(INTEGRATION_TESTS) -c $(CONFIGURATION) -m:1 -p:BuildInParallel=false -nr:false

lint:
	@for project in $(RUNTIME_PROJECT) $(HOSTING_PROJECT) $(TESTING_PROJECT) $(UNIT_TESTS) $(INTEGRATION_TESTS); do \
		$(DOTNET) restore $$project; \
		$(DOTNET) format $$project --verify-no-changes --verbosity minimal --no-restore; \
	done

clean:
	@for project in $(RUNTIME_PROJECT) $(HOSTING_PROJECT) $(TESTING_PROJECT) $(UNIT_TESTS) $(INTEGRATION_TESTS); do \
		$(DOTNET) clean $$project -c $(CONFIGURATION) -m:1 -p:BuildInParallel=false -nr:false; \
	done
	rm -rf $(PACKAGE_OUTPUT)

build:
	$(DOTNET) build $(SOLUTION) -c $(CONFIGURATION) -p:TreatWarningsAsErrors=true -p:BuildInParallel=false -m:1 -nr:false -v minimal

publish:
	$(DOTNET) pack $(RUNTIME_PROJECT) -c $(CONFIGURATION) -o $(PACKAGE_OUTPUT) -p:BuildInParallel=false -m:1 -nr:false
