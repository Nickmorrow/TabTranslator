using System;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TabTranslator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = @"..\..\..\..\JSONFiles";

            //Defining SixStringGuitar

            // Standard Tuning

            MusicString GString0 = new MusicString();
            GString0.Tuning = RootNotes.E;
            MusicString GString1 = new MusicString();
            GString1.Tuning = RootNotes.B;
            MusicString GString2 = new MusicString();
            GString2.Tuning = RootNotes.G;
            MusicString GString3 = new MusicString();
            GString3.Tuning = RootNotes.D;
            MusicString GString4 = new MusicString();
            GString4.Tuning = RootNotes.A;
            MusicString GString5 = new MusicString();
            GString5.Tuning = RootNotes.E;

            List<MusicString> StandardGuitarTunings = new List<MusicString>(); //string list is reverse of normal to match json data
            StandardGuitarTunings.Add(GString0);
            StandardGuitarTunings.Add(GString1);
            StandardGuitarTunings.Add(GString2);
            StandardGuitarTunings.Add(GString3);
            StandardGuitarTunings.Add(GString4);
            StandardGuitarTunings.Add(GString5);

            StringInstrument SixStringGuitar = new StringInstrument();

            SixStringGuitar.Name = "SixStringGuitar";
            SixStringGuitar.FretCount = 21;
            SixStringGuitar.MusicStrings = StandardGuitarTunings;

            // Defining BassGuitar

            // Standard Tuning

            MusicString BGString0 = new MusicString();
            BGString0.Tuning = RootNotes.G;
            MusicString BGString1 = new MusicString();
            BGString1.Tuning = RootNotes.D;
            MusicString BGString2 = new MusicString();
            BGString2.Tuning = RootNotes.A;
            MusicString BGString3 = new MusicString();
            BGString3.Tuning = RootNotes.E;

            List<MusicString> StandardBassGuitarTunings = new List<MusicString>();
            StandardBassGuitarTunings.Add(BGString0);
            StandardBassGuitarTunings.Add(BGString1);
            StandardBassGuitarTunings.Add(BGString2);
            StandardBassGuitarTunings.Add(BGString3);

            StringInstrument BassGuitar = new StringInstrument();

            BassGuitar.Name = "BassGuitar";
            BassGuitar.FretCount = 21;
            BassGuitar.MusicStrings = StandardBassGuitarTunings;

            // Defining Ukelele

            // Standard tuning

            MusicString UkString0 = new MusicString();
            UkString0.Tuning = RootNotes.A;
            MusicString UkString1 = new MusicString();
            UkString1.Tuning = RootNotes.E;
            MusicString UkString2 = new MusicString();
            UkString2.Tuning = RootNotes.C;
            MusicString UkString3 = new MusicString();
            UkString3.Tuning = RootNotes.G;

            List<MusicString> StandardUkeleleTunings = new List<MusicString>();
            StandardUkeleleTunings.Add(BGString0);
            StandardUkeleleTunings.Add(BGString1);
            StandardUkeleleTunings.Add(BGString2);
            StandardUkeleleTunings.Add(BGString3);

            StringInstrument Ukelele = new StringInstrument();

            Ukelele.Name = "Ukelele";
            Ukelele.FretCount = 12;
            Ukelele.MusicStrings = StandardUkeleleTunings;


            // **TESTS**

            List<SongsterrSong> Songs = GetJsonSongs(path);
            List<MusicalBeat> songBeats = GetSongBeats(Songs[3], Ukelele);

            var tab = new Tab(Songs[3], Ukelele, songBeats);

            List<string> tabOne = tab.TabLines[0];
            int tabLength = tabOne.Count;
            int measuresPerLine = 10;
            int tabLineStartPoint = 0;
            int tabLineEndPoint = measuresPerLine;

            Console.WriteLine($"{tab.TitleOfSong}\n{tab.InstrumentString}");
            foreach(long tuning in tab.Tuning)
            {
                Console.Write(tuning.ToString());
            }
            Console.WriteLine($"\nCapo on Fret {tab.Capo.ToString()}\n");

            while (tabLineStartPoint < tabLength)
            {
                int remainingMeasures = tabLength - tabLineEndPoint;
                for (int i = 0; i < tab.TabLines.Count; i++)
                {
                    List<string> tabLine = tab.TabLines[i];
                    for (int h = tabLineStartPoint; h < tabLineEndPoint; h++)
                    { 
                        string measure = tabLine[h];
                        int dashCount = measure.Length;

                        for (int k = 0; k < dashCount; k++)
                        {
                            Console.Write(measure[k]);
                        }
                    }
                    Console.Write($"\n");
                    //       File.AppendAllText(@"c:\test.txt", "lol");
                }
                tabLineStartPoint += 10;
                if (remainingMeasures >= 10)
                {
                    tabLineEndPoint += 10;
                }
                else
                {
                    tabLineEndPoint = tabLength;
                }
                Console.Write($"\n");
            }
        }
        /// <summary>
        /// Gets the notes, duration. and octave from songster json
        /// </summary>
        /// <param name="song"></param>
        /// <param name="stringInstrument"></param>
        /// <returns>list of MusicalNotes</returns>
        public static List<MusicalBeat> GetSongBeats(SongsterrSong song, StringInstrument stringInstrument)
        {

            List<MusicalBeat> beats = new List<MusicalBeat>();

            for (int measureNum = 0; measureNum < song.Measures.Count(); measureNum++)
            {

                for (int voiceNum = 0; voiceNum < song.Measures[measureNum].Voices.Count(); voiceNum++)
                {
                    for (int beatNum = 0; beatNum < song.Measures[measureNum].Voices[voiceNum].Beats.Count(); beatNum++)
                    {
                        MusicalBeat beat = new MusicalBeat();
                        List<MusicalNote> notes = new List<MusicalNote>();
                        beat.SongsterrDuration = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Duration[1];
                        beat.Duration16ths = MusicalBeat.Get16ths(beat.SongsterrDuration);
                        beat.NullableBool = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Rest;
                        beat.IsRest = MusicalBeat.GetRestBeat(beat.NullableBool);

                        for (int noteNum = 0; noteNum < song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes.Count(); noteNum++)
                        {
                            MusicalNote note = new MusicalNote();
                            note.FingerPosition.StringNum = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes[noteNum].String;
                            note.FingerPosition.FretNr = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes[noteNum].Fret;
                            note.RootNote = MusicalNote.GetRootNote(note.FingerPosition, stringInstrument.MusicStrings[Convert.ToInt32(note.FingerPosition.StringNum)]);
                            note.Octave = MusicalNote.GetOctave(note.FingerPosition);
                            note.NullableBool = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes[noteNum].Rest;
                            note.IsRest = MusicalNote.GetRestNote(note.NullableBool);
                            notes.Add(note);
                        }
                        beat.MusicalNotes = notes;
                        beats.Add(beat);
                    }
                }
            }

            return beats;
        }

        /// <summary>
        /// Converts directory of json files into list of objects
        /// </summary>
        /// <param name="dPath"></param>
        /// <returns>List of objects</returns>
        public static List<SongsterrSong> GetJsonSongs(string dPath)
        {
            List<SongsterrSong> Songs = new List<SongsterrSong>();
            DirectoryInfo dir = new DirectoryInfo(dPath);
            string[] fPaths = Directory.GetFiles(dPath);
            string json = "";
            int fileNum = fPaths.Count();

            for (int i = 0; i < fileNum; i++)
            {
                json = File.ReadAllText(fPaths[i]);
                SongsterrSong song = JsonConvert.DeserializeObject<SongsterrSong>(json);
                Songs.Add(song);
            }

            return Songs;
        }


    }
}
