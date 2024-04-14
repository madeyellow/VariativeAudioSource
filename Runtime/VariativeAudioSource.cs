using UnityEngine;

namespace MadeYellow.Audios
{
	[RequireComponent(typeof(AudioSource))]
	public class VariativeAudioSource : MonoBehaviour
	{
		[SerializeField]
		private VariativeAudioSourceMode _mode;
		public VariativeAudioSourceMode Mode { get => _mode; set => _mode = value; }

		[Space]

		[SerializeField]
		private AudioClip[] _audioClips;
		public AudioClip[] AudioClips { get => _audioClips; set => _audioClips = value; }

		private AudioSource _audioSource;

		private int _lastAudioClipIndex = -1;

		[Space]

		[SerializeField]
		private bool _usePitchDeviation = true;
		public bool UsePitchDeviation { get => _usePitchDeviation; set => _usePitchDeviation = value; }

		[Range(0f, 1f)]
		[SerializeField]
		private float _pitchDeviation = 0.05f;
		public float PitchDeviation { get => _pitchDeviation; set => _pitchDeviation = value; }

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		/// <summary>
		/// Plays one AudioClip from an array of AudioClips, and scales the AudioSource volume by volumeScale.
		/// </summary>
		/// <param name="volumeScale">The scale of the volume (0-1).</param>
		public void Play(float volumeScale)
		{
			var audioClip = GetAudioClip();

			if (audioClip != null)
			{
				PitchShifting();

				_audioSource.PlayOneShot(audioClip, volumeScale);
			}
		}

		public void Play()
		{
			Play(1f);
		}

		private AudioClip GetAudioClip()
		{
			if (_audioClips.Length == 0)
			{
				return null;
			}

			switch (_mode)
			{
				case VariativeAudioSourceMode.Random:
					return GetRandomAudioClip();

				default:
					return GetNextAudioClip();
			}
		}

		// Функция подбирающая звук для воспроизведения
		private AudioClip GetNextAudioClip()
		{
			// Перейти к следующему по порядку аудиоклипу
			_lastAudioClipIndex++;

			// Если вышли за границы массива, то вернуться к первой записи
			if (_lastAudioClipIndex >= _audioClips.Length)
			{
				_lastAudioClipIndex = 0;
			}

			return _audioClips[_lastAudioClipIndex];
		}

		private AudioClip GetRandomAudioClip()
		{
			// If there are only 2 or 1 audioclip - no need to pick random
			if (_audioClips.Length <= 2)
			{
				return GetNextAudioClip();
			}
			else
			{
				var randomIndex = 0;

				do
				{
					randomIndex = Random.Range(0, _audioClips.Length - 1);
				} while (randomIndex == _lastAudioClipIndex);

				_lastAudioClipIndex = randomIndex;
			}

			return _audioClips[_lastAudioClipIndex];
		}

		private void PitchShifting()
		{
			// Check if use of pitch is allowed
			if (_usePitchDeviation && _pitchDeviation > 0)
			{
				var pitchShift = Random.Range(-_pitchDeviation, _pitchDeviation);

				_audioSource.pitch = 1f + pitchShift;
			}
		}
	}

	public enum VariativeAudioSourceMode
	{
		/// <summary>
		/// Play ordered sequence of sounds
		/// </summary>
		Ordered,

		/// <summary>
		/// Pick random sounds
		/// </summary>
		Random
	}
}