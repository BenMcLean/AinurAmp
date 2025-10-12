window.audioPlayer = {
	currentSound: null,

	playFlac: function (trackId) {
		if (this.currentSound) {
			this.currentSound.unload();
		}

		this.currentSound = new Howl({
			src: [`/api/audio/${trackId}`],
			format: ['flac'],
			html5: true, // Enable streaming
			onload: function () {
				console.log('FLAC loaded');
			},
			onplay: function () {
				console.log('Playing');
			},
			onend: function () {
				console.log('Track ended');
			},
			onerror: function (id, error) {
				console.error('Playback error:', error);
			}
		});

		this.currentSound.play();
	},

	pause: function () {
		if (this.currentSound) {
			this.currentSound.pause();
		}
	},

	stop: function () {
		if (this.currentSound) {
			this.currentSound.stop();
		}
	},

	seek: function (seconds) {
		if (this.currentSound) {
			this.currentSound.seek(seconds);
		}
	},

	getPosition: function () {
		return this.currentSound ? this.currentSound.seek() : 0;
	},

	getDuration: function () {
		return this.currentSound ? this.currentSound.duration() : 0;
	}
};
