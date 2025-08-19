namespace PRO.SkillEditor
{
    public class Track_Disk
    {
        public SkillVisual_Disk.PlayTrack TrackEnum;
        public Slice_DiskBase[] SlickArray;

        public Track_Disk(SkillVisual_Disk.PlayTrack track, int length)
        {
            TrackEnum = track;
            SlickArray = new Slice_DiskBase[length];
        }
    }
}
