/// <summary>
///     Noise interface defines the required method for a Noise object
/// </summary>
public interface Noise
{
    /// <summary>
    ///     sample method gets a sample from a noise object
    /// </summary>
    /// <param name="pos">float array of position of sample</param>
    /// <returns>float of sample value</returns>
    float sample(float[] pos);

    /// <summary>
    ///     toString method returns a repsentation of noise in a string format
    /// </summary>
    /// <returns>string repsentation of noise</returns>
    string toString();
}
