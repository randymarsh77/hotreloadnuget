const { exec } = require('shelljs');

const version = process.argv.length >= 4 && process.argv[2] === '--version' && process.argv[3];
if (!version) {
	console.error('You need to pass a version to publish.');
	process.exit(1);
}

const apiKey = process.env.NUGET_API_KEY;
if (!version) {
	console.error('You need to configure NUGET_API_KEY.');
	process.exit(1);
}

(async () => {
	try {
		await verifyExec(
			`dotnet pack packages/hotreloadnuget --configuration Release --output release -p:PackageVersion=${version}`
		);

		await verifyExec(
			`dotnet nuget push "release/*.nupkg" -k ${apiKey} -s https://api.nuget.org/v3/index.json --skip-duplicate --no-symbols`
		);
	} catch (e) {
		console.error(e);
		process.exit(1);
	}
})();

async function verifyExec(command) {
	const { code } = await exec(command);
	if (code !== 0) {
		process.exit(code);
	}
}
