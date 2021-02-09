module.exports = {
	branches: ['master'],
	tagFormat: 'hotreloadnuget@${version}',
	plugins: [
		"@semantic-release/commit-analyzer",
		"@semantic-release/release-notes-generator",
		[
			'@semantic-release/exec',
			{
				publishCmd: 'node publish.js --version ${nextRelease.version}',
			},
		],
	],
};
