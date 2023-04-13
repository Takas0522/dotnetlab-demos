module.exports = {
  pipeline: {
    build: {
      dependsOn: ["^build"],
      outputs: [ "dist" ]
    },
    lint: [ "^lint" ],
    test: {
      dependsOn: ["build","^test"],
      outputs: [ "dist" ]
    }
  }
};