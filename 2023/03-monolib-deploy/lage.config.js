module.exports = {
  pipeline: {
    build: [ "^build" ],
    lint: [ "^lint" ],
    test: [ "build","^test" ]
  },
  writeRemoteCache: true
};