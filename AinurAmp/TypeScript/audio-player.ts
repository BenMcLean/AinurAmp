import { Howl, HowlOptions } from 'howler';

interface AudioPlayer {
	currentSound: Howl | null;
	playFlac(trackId: string): void;
	pause(): void;
	stop(): void;
	seek(seconds: number): void;
	getPosition(): number;
	getDuration(): number;
}

const audioPlayer: AudioPlayer = {
	currentSound: null,

	playFlac(trackId: string): void {
		if (this.currentSound) {
			this.currentSound.unload();
		}

		const options: HowlOptions = {
			src: [`/api/audio/${trackId}`],
			format: ['flac'],
			html5: true,
			onload: () => {
				console.log('FLAC loaded');
			},
			onplay: () => {
				console.log('Playing');
			},
			onend: () => {
				console.log('Track ended');
			},
			onloaderror: (soundId: number, error: any) => {
				console.error('Load error:', soundId, error);
			},
			onplayerror: (soundId: number, error: any) => {
				console.error('Playback error:', soundId, error);
			}
		};

		this.currentSound = new Howl(options);
		this.currentSound.play();
	},

	pause(): void {
		if (this.currentSound) {
			this.currentSound.pause();
		}
	},

	stop(): void {
		if (this.currentSound) {
			this.currentSound.stop();
		}
	},

	seek(seconds: number): void {
		if (this.currentSound) {
			this.currentSound.seek(seconds);
		}
	},

	getPosition(): number {
		return this.currentSound ? this.currentSound.seek() as number : 0;
	},

	getDuration(): number {
		return this.currentSound ? this.currentSound.duration() : 0;
	}
};

// Expose to window for Blazor JS interop
declare global {
	interface Window {
		audioPlayer: AudioPlayer;
	}
}

window.audioPlayer = audioPlayer;
