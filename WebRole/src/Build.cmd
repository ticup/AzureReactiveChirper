
@setlocal
node node_modules\browserify\bin\cmd.js --outfile ../Scripts/chirper.min.js -t [babelify --presets [es2015 react] ]