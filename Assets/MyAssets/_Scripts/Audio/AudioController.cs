using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace ColdClimb.Audio{
    public class AudioController : MonoBehaviour{
        // Members
        public static AudioController instance;

        public bool debug;
        public AudioTrack[] tracks;

        private Hashtable audioTable; // relationship between audio types (key) and audio tracks (value)
        private Hashtable jobTable; // Relationship between audio types (key) and jobs (value) (Coroutine, IEnumator)
                                    // Each job is relating to the audio (PlayAudio, StopAudio, RestartAudio)

        [System.Serializable]
        public class AudioObject{
            public AudioType type;
            public AudioClip clip;
        }

        [System.Serializable]
        public class AudioTrack{
            public AudioSource globalSource;
            public AudioObject[] audio;
            public AudioMixerGroup audioMixer;
        }

        private class AudioJob{
            public AudioAction action;
            public AudioType type;
            public bool fade;
            public float delay;
            public float fadeDuration;
            public AudioSource audioSource;

            public AudioJob(AudioAction jobAction, AudioType audioType, bool includeFade, float _fadeDuration, float audioDelay, AudioSource source){
                action = jobAction;
                type = audioType;
                fade = includeFade;
                fadeDuration = _fadeDuration;
                delay = audioDelay;
                audioSource = source;
            }
        }

        private enum AudioAction{
            START,
            STOP,
            RESTART,
            LOOP
        }

#region Unity Functions
        private void Awake() {
            if(!instance){
                Configure();
            }
        }

        private void OnDisable() {
            Dispose();
        }
#endregion

#region Public Functions
        public void PlayAudio(AudioType type, bool fade = false, float fadeDuration = 1.0f, float delay = 0.0f, AudioSource source = null){
            AddJob(new AudioJob(AudioAction.START, type, fade, fadeDuration, delay, source));
        }
        public void PlayLoopingAudio(AudioType type, bool fade = false, float fadeDuration = 1.0f, float delay = 0.0f, AudioSource source = null){
            AddJob(new AudioJob(AudioAction.LOOP, type, fade, fadeDuration, delay, source));
        }
        public void StopAudio(AudioType type, bool fade = false, float fadeDuration = 1.0f, float delay = 0.0f, AudioSource source = null){
            AddJob(new AudioJob(AudioAction.STOP, type, fade, fadeDuration, delay, source));
        }
        public void RestartAudio(AudioType type, bool fade = false, float fadeDuration = 1.0f, float delay = 0.0f, AudioSource source = null){
            AddJob(new AudioJob(AudioAction.RESTART, type, fade, fadeDuration, delay, source));
        }
#endregion

#region Private Functions
        private void Configure(){
            instance = this;
            audioTable = new Hashtable();
            jobTable = new Hashtable();
            GenerateAudioTable();
        }
        private void Dispose(){
            foreach(DictionaryEntry entry in jobTable){
                IEnumerator job = (IEnumerator) entry.Value;
                StopCoroutine(job);
            }
        }

        private void GenerateAudioTable(){
            foreach (AudioTrack track in tracks){
                if(track.audioMixer != null && track.globalSource != null){
                    track.globalSource.outputAudioMixerGroup = track.audioMixer;
                }
                foreach (AudioObject audioObject in track.audio){
                    // Do not duplicate keys
                    if(audioTable.ContainsKey(audioObject.type)){
                        LogWaring($"You are trying to register audio [{audioObject.type}] that has already been registered.");
                    }
                    else{
                       audioTable.Add(audioObject.type, track);
                       Log($"Registering audio [{audioObject.type}]."); 
                    }
                }
            }
        }

        private void AddJob(AudioJob job){
            // remove confliction jobs
            RemoveConflictingJobs(job.type);

            // start job
            IEnumerator jobRunner = RunAudioJob(job);
            jobTable.Add(job.type, jobRunner);
            StartCoroutine(jobRunner);
            Log($"Starting job on [{job.type}] with operation: {job.action}");
        }

        private void RemoveJob(AudioType type){
            if(!jobTable.ContainsKey(type)){
                LogWaring($"Trying to stop a job [{type}] that is not running.");
                return;
            }

            IEnumerator runningJob = (IEnumerator)jobTable[type];
            StopCoroutine(runningJob);
            jobTable.Remove(type);
        }

        private IEnumerator RunAudioJob(AudioJob job){
            yield return new WaitForSeconds(job.delay);

            AudioTrack track = (AudioTrack)audioTable[job.type];
            AudioSource audioSourceToUse = job.audioSource == null ? track.globalSource : job.audioSource;

            if(audioSourceToUse == null){
                LogWaring($"No audio source found on track: [{track}] and job: [{job.action}], returning...");
                yield return null;
            }

            if(audioSourceToUse.outputAudioMixerGroup == null && track.audioMixer != null){
                audioSourceToUse.outputAudioMixerGroup = track.audioMixer;
            }

            audioSourceToUse.clip = GetAudioClipFromAudioTrack(job.type, track);

            //if we are looping set the source loop to true
            audioSourceToUse.loop = job.action == AudioAction.LOOP;

            switch (job.action){
                case AudioAction.START:
                    audioSourceToUse.Play();
                    break;
                case AudioAction.STOP:
                    if (!job.fade)
                    {
                        audioSourceToUse.Stop();
                    }
                    break;
                case AudioAction.RESTART:
                    audioSourceToUse.Stop();
                    audioSourceToUse.Play();
                    break;
                case AudioAction.LOOP:
                    audioSourceToUse.Play();
                    break;
            }

            if (job.fade){
                float initial = job.action == AudioAction.START || job.action == AudioAction.RESTART || job.action == AudioAction.LOOP ? 0.0f : 1.0f;
                float target = initial == 0 ? 1 : 0;
                float duration = job.fadeDuration;
                float timer = 0;

                while(timer <= duration){
                    audioSourceToUse.volume = Mathf.Lerp(initial, target, timer / duration);
                    timer += Time.deltaTime;
                    yield return null;
                }

                if(job.action == AudioAction.STOP){
                    audioSourceToUse.Stop();
                }
            }

            jobTable.Remove(job.type);
            Log($"Job count: {jobTable.Count}");

            yield return null;
        }

        private void RemoveConflictingJobs(AudioType type){
            // Conflict: if we are already using the type
            if(jobTable.ContainsKey(type)){
                RemoveJob(type);
            }

            // Conflict: if we want to play a audio type on the same audio track that another audio type is playing on
            AudioType conflictAudio = AudioType.None;
            foreach (DictionaryEntry entry in jobTable){
                AudioType audioType = (AudioType)entry.Key;
                AudioTrack audioTrackInUse = (AudioTrack)audioTable[audioType];
                AudioTrack audioTrackNeeded = (AudioTrack)audioTable[type];
                if(audioTrackNeeded == audioTrackInUse){
                    // conflict
                    conflictAudio = audioType;
                }
            }
            if(conflictAudio != AudioType.None){
                RemoveJob(conflictAudio);
            }
        }

        private AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track){
            foreach (AudioObject audioObject in track.audio){
                if(audioObject.type == type){
                    return audioObject.clip;
                }
            }
            return null;
        }

        private void Log(string msg){
            if(!debug) return;
            Debug.Log("[Audio Controller]: " + msg);
        }
        private void LogWaring(string msg){
            if(!debug) return;
            Debug.LogWarning("[Audio Controller]: " + msg);
        }
        #endregion
    }
}